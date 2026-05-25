document.addEventListener('DOMContentLoaded', () =>
{
    const synth = window.speechSynthesis;

    const textInput = document.getElementById("text-input");
    const textContainer = document.getElementById("text-container");
    const voiceSelect = document.getElementById("voice-select");
    const rateSelect = document.getElementById("rate-select");

    const btnPlay = document.getElementById("btn-play");
    const btnPause = document.getElementById("btn-pause");
    const btnStop = document.getElementById("btn-stop");
    const btnPrev = document.getElementById("btn-prev");
    const btnNext = document.getElementById("btn-next");
    const statusText = document.getElementById("Status");

    let rawSegments = [];
    let currentSentenceIndex = 0;
    let IsPaused = false;
    let IsPlaying = false;
    let IsStoppping = false;
    let availableVoices = [];
    let voicesLoadPromise = null;

    btnPrev.disabled = true;
    btnNext.disabled = true;

    function getRussianVoicesFirst(voice)
    {
        const ruVoices = voice.filter(x => x.lang.toLowerCase().startsWith('ru'));
        const otherVoices = voice.filter(x => !x.lang.toLowerCase().startsWith('ru'));
        return [...ruVoices, ...otherVoices];
    }

    function renderVoice(voices)
    {
        const selectedVoiceName = voiceSelect.value;

        voiceSelect.innerHTML = '';

        if (voices.length === 0)
        {
            const option = document.createElement('option');

            option.textContent = 'Voices not found, chech windows voices';
            option.value = '';

            voiceSelect.appendChild(option);
            voiceSelect.disabled = true;

            return;
        }

        voiceSelect.disabled = true;


        getRussianVoicesFirst(voices).forEach(voice => {
            const option = document.createElement('option');

            option.textContent = `${voice.name} (${voice.lang})`;
            option.value = voice.name;

            voiceSelect.appendChild(option);
        });

        const preferedVoice = voice.find(x => x.name == selectedVoiceName) ?? voices.find(x => x.lang.toLowerCase().startsWith('ru'));
        if (preferedVoice)
        {
            voiceSelect.value = preferedVoice.name;
        }
    }

    function loadVoices()
    {
        availableVoices = synth.getVoices();
        renderVoice(availableVoices);

        return availableVoices
    }

    function waitForVoice()
    {
        if (voicesLoadPromise) {
            return voicesLoadPromise;
        }

        voicesLoadPromise = new Promise(resolve =>
        {
            const maxAttempts = 20;
            let attempts = 0;

            const tryLoad = () => {
                const voices = loadVoices();

                if (voices.length === 0 || attempts > maxAttempts) {
                    resolve(voices);
                    return;
                }

                attempts++;
                window.setTimeout(tryLoad, 250);
            }
            tryLoad();
        });
        return voicesLoadPromise;
    }

    if (!synth) {
        voiceSelect.innerHTML = '<option value="">Syntes is not support</option>'
        voiceSelect.disabled = true;

        statusText.textContent = "Your browser is not support Web Speech API";

        btnPlay.disabled = true;
        btnPause.disabled = true;
        btnStop.disabled = true;
        btnPrev.disabled = true;
        btnNext.disabled = true;

        return;
    }

    if (typeof synth.addEventListener === "function") {
        synth.addEventListener('voiceschanged', loadVoices);
    }
    else {
        synth.onvoiceschanged = loadVoices;
    }

    waitForVoice();

    function prepareTextDisplay(text) {
        rawSegments = [];
        currentSentenceIndex = 0;
        textContainer.innerHTML = '';

        if (!text.trim()) {
            textContainer.textContent = 'Enter text for synthes';
            return;
        }
        const regEx = "/([.!?\n]+)/";

        const parts = text.split(regEx);

        for (let i = 0; i < text.length; i++) {
            if (!parts[i]) {
                continue;
            }

            if (regEx.test(parts[i])) {
                if (rawSegments.length > 0) {
                    rawSegments[rawSegments.length - 1] += parts[i];
                }
                else {
                    rawSegments.push(parts[i]);
                }
            }
            else {
                rawSegments.push(parts[i]);
            }
        }

        rawSegments.forEach((segment, index) =>
        {
            const span = document.createElement('span');

            span.id = `seg-${index}`;
            span.textContent = segment;

            textContainer.append(span);
        });
    }


    function clearHighlight() {
        const oldHighlight = textContainer.querySelector('.highlight');

        if (oldHighlight) {
            oldHighlight.classList.remove('.highlight');
        }
    }

    function updateNavigationButtons() {
        btnPrev.disabled = !IsPlaying || currentSentenceIndex <= 0;
        btnNext.disabled = !IsPlaying || currentSentenceIndex >= rawSegments.length - 1;
    }

    function updateHighlight(index) {
        clearHighlight();

        const currentSpan = document.getElementById(`seg-${index}`);

        if (currentSpan) {
            currentSpan.classList.add('highlight');

            currentSpan.scrollIntoView(
                {
                    behavior: 'smooth',
                    block: 'nearest'
                });
        }
        updateNavigationButtons();
    }

    async function saveSpeechLog() {
        const data =
        {
            text: textInput.value,
            voiceName: voiceSelect.value,
            rate: parseFloat(rateSelect.value)
        };

        try {
            const response = await fetch('/api/speechlogs',
                {
                    method: 'POST',
                    headers:
                    {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                });
            return;
        }
        catch {
            return false;
        }
    }

    function finishReading(message) {
        IsPaused = false;
        IsPlaying = false;
        IsStoppping = false;

        statusText.textContent = message;
        updateNavigationButtons();
    }

    function speakCurrentSentence() {
        if (!IsPlaying) {
            return;
        }

        if (currentSentenceIndex >= rawSegments.length) {
            finishReading('Read is over');
        }

        const textSegment = rawSegments[currentSentenceIndex].trim();
        if (!textSegment) {
            currentSentenceIndex++;
            speakCurrentSentence();
            return;
        }

        updateHighlight(currentSentenceIndex);
        const utterance = new SpeechSynthesisUtterance(textSegment);
        utterance.voice = availableVoices.find(x => x.name === voiceSelect.value) ?? synth.getVoices().find(x => x.name === voiceSelect.value) ?? null;

        utterance.rate = parseFloat(rateSelect.value);

        utterance.onend = () =>
        {
            if (IsStoppping || !IsPlaying || IsPaused) {
                return;
            }

            currentSentenceIndex++;
            speakCurrentSentence();
        };

        utterance.onerror = () => {
            if (IsStoppping) {
                return;
            }

            finishReading("Error by speaking");
        };

        statusText.textContent = `Read part: ${currentSentenceIndex + 1} / ${rawSegments.length}`;
        synth.speak(utterance);
    }

    btnPlay.addEventListener("click", async () =>
    {
        await waitForVoice();

        if (IsPaused) {
            synth.resume();
            IsPaused = false;
            statusText.textContainer = 'Continue reading';
            updateNavigationButtons();
            return;
        }

        IsStoppping = true;
        synth.cancel();

        prepareTextDisplay(textInput.value);
        IsStoppping = false;
        IsPaused = false;

        if (rawSegments.length === 0) {
            finishReading("Enter text");
        }

        IsPlaying = true;
        updateNavigationButtons();

        const isLogSaved = await saveSpeechLog();
        if (!isLogSaved) {
            statusText.textContent = 'Reading without saving logs';
        }
    });

    btnPause.addEventListener("click", async () =>
    {
        if (synth.speaking && !IsPaused) {
            synth.pause();
            IsPaused = true;
            statusText.textContent = 'Pause';
            updateNavigationButtons();
        }
    });

    btnStop.addEventListener("click", async () =>
    {
        IsStoppping = true;
        IsPlaying = false;
        IsPaused = false;

        synth.cancel();
        currentSentenceIndex = 0;
        clearHighlight();
        statusText.textContent = "Stopped";
        updateNavigationButtons();
        window.setTimeout(() =>
        {
            IsStoppping = false;
        }, 0);
    });

    btnNext.addEventListener("click", async () =>
    {
        if (!IsPlaying || currentSentenceIndex >= rawSegments.length - 1) {
            return;
        }

        IsStoppping = true;
        synth.cancel();
        IsPaused = false;
        currentSentenceIndex++;
        window.setTimeout(() => {
            IsStoppping = false;
            speakCurrentSentence();
        }, 0);
    });

    btnPrev.addEventListener("click", async () =>
    {
        if (!IsPlaying || currentSentenceIndex <= 0) {
            return;
        }

        IsStoppping = true;
        synth.cancel();
        IsPaused = false;
        currentSentenceIndex--;
        window.setTimeout(() => {
            IsStoppping = false;
            speakCurrentSentence();
        }, 0);
    });
});