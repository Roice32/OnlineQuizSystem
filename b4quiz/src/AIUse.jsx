import { useState } from 'react';
import Navbar from './components/Navbar';

const AIUse = () => {
    const [inputValue, setInputValue] = useState('');
    const [difficultyValue, setDifficultyValue] = useState('');
    const [numQuestions, setNumQuestions] = useState('');
    const [questionType, setQuestionType] = useState('multiple_choice');
    const [responseText, setResponseText] = useState('');

    const handleClick = async () => {
        try {
            const tema = inputValue;
            let mesaj = '';
            if (questionType === 'multiple_choice') {
                mesaj = `Vreau să creezi ${numQuestions} întrebări de dificultate ${difficultyValue}, fiecare având 4 variante de răspuns, una corectă și 3 greșite, pentru un quiz cu tema "${tema}". Vei returna raspunsul sub forma de json, unde intrebarea va fi un string, iar variantele de raspuns vor fi un array de stringuri. Varianta corecta va fi prima varianta posibila.
                Exemplu: {
                    "1": {
                        "intrebare": "Cine a fost primul rege al Angliei?",
                        "variante": [
                            "William I Cuceritorul",
                            "Henry VIII",
                            "Elizabeth I",
                            "Richard III"
                        ]
                    }
                }`;
                

            } else if (questionType === 'true_false') {
                mesaj = `Vreau să creezi ${numQuestions} întrebări de dificultate ${difficultyValue}, fiecare având ca raspuns true sau false, pentru un quiz cu tema "${tema}". Vei returna raspunsul sub forma de json, unde intrebarea va fi un string, iar varianta de raspuns true sau false. Ca si in exemplul dat.
                Exemplu: {
                    "1": {
                        "intrebare": "Romania a luptata in al doilea razboi mondial?",
                        "variante": [
                            "True"
                        ]
                    }
                }`;         
            }

            const response = await fetch('https://api.textcortex.com/v1/codes', {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer gAAAAABmOiEDhlUAHKGx2bE9D5STmMvKgsuM2FaNLHVZ3_OWSGEJvhsPCfyztsWqT9V-03iE-uoHVSoRVZgdTeW593DH7j2Uc1ZLBIe_ySogrTb91Sjq72zVyzZc6KwBXzvBdTJ19AwZ',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    "max_tokens": 2048,
                    "mode": "python",
                    "model": "icortex-1",
                    "n": 1,
                    "temperature": 0,
                    "text": mesaj
                })
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const responseData = await response.json();
            console.log('Răspunsul API:', responseData);
            const formattedResponse = formatResponse(responseData.data.outputs[0].text);
            setResponseText(formattedResponse);
        } catch (error) {
            console.error('Eroare la efectuarea cererii:', error);
        }
    };

    const formatResponse = (rawResponse) => {
        const responseObj = JSON.parse(rawResponse);
        let formattedResponse = '';
        Object.keys(responseObj).forEach((key, index) => {
            const question = responseObj[key];
            formattedResponse += `<p>${index + 1}. ${question.intrebare}</p>`;
            question.variante.forEach((variant, idx) => {
                const variantLabel = String.fromCharCode(97 + idx); // a, b, c, d
                const color = idx === 0 ? 'green' : 'inherit';
                formattedResponse += `<p style="color: ${color};">${variantLabel}) ${variant}</p>`;
            });
        });
        return formattedResponse;
    };

    return (
        <div>
            <div>
                <Navbar />
                <div className='flex flex-col items-center'>
                    <h1 className='text-[#DEAE9F] text-4xl font-bold mt-6'>AI</h1>
                </div>
            </div>

            <div className='flex flex-col items-center'>
                <label className='text-white text-lg mt-6'>Dificultate:</label>
                <select
                    value={difficultyValue}
                    onChange={(e) => setDifficultyValue(e.target.value)}
                    className='border border-gray-400 rounded-md px-3 py-1 mt-2'
                >
                    <option value="">Selectează...</option>
                    <option value="ușoară">Ușoară</option>
                    <option value="mediu">Mediu</option>
                    <option value="dificilă">Dificilă</option>
                </select>

                <label className='text-white text-lg mt-4'>Nr. întrebări:</label>
                <input 
                    type="number"
                    value={numQuestions}
                    onChange={(e) => setNumQuestions(e.target.value)}
                    className='border border-gray-400 rounded-md px-3 py-1 mt-2'
                />

                <label className='text-white text-lg mt-4'>Tema:</label>
                <input 
                    value={inputValue}
                    onChange={(e) => setInputValue(e.target.value)}
                    className='border border-gray-400 rounded-md px-3 py-1 mt-2'
                />

                <label className='text-white text-lg mt-4'>Tip întrebări:</label>
                <select
                    value={questionType}
                    onChange={(e) => setQuestionType(e.target.value)}
                    className='border border-gray-400 rounded-md px-3 py-1 mt-2'
                >
                    <option value="multiple_choice">4 variante de răspuns</option>
                    <option value="true_false">Adevărat/Fals</option>
                </select>

                <span>
                    <button 
                        className="mt-5 text-white bg-blue-500 border border-blue-500 rounded-md px-3 py-1"
                        onClick={handleClick}
                    >
                        OK
                    </button>
                </span>
            </div>
            {responseText && (
                <div className="mt-4 p-4 border border-gray-400 rounded-md text-white">
                    <h2 className="text-lg font-bold mb-2">Răspunsul AI:</h2>
                    <div dangerouslySetInnerHTML={{ __html: responseText }} />
                </div>
            )}
        </div>
    ); 
}

export default AIUse;
