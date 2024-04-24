import React, { useState } from 'react';
import Navbar from '../components/Navbar';

const QuizCreate = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [imageUrl, setImageUrl] = useState('');
    const [language, setLanguage] = useState('Romana');
    const [timeLimitMinutes, setTimeLimitMinutes] = useState(0);
    const [questions, setQuestions] = useState([{ id: 1, text: '', type: 'trueFalse', options: ['', ''] }]);
    const [nextId, setNextId] = useState(2);

    const handleNameChange = (event) => {
        setName(event.target.value);
    };

    const handleDescriptionChange = (event) => {
        setDescription(event.target.value);
    };

    const handleImageChange = (event) => {
        setImageUrl(event.target.value);
    };

    const handleLanguageChange = (event) => {
        setLanguage(event.target.value);
    };

    const handleTimeLimitChange = (event) => {
        setTimeLimitMinutes(parseInt(event.target.value));
    };

    const handleQuestionChange = (index, event) => {
        const newQuestions = [...questions];
        newQuestions[index].text = event.target.value;
        setQuestions(newQuestions);
    };

    const handleQuestionTypeChange = (index, event) => {
        const newQuestions = [...questions];
        newQuestions[index].type = event.target.value;
        newQuestions[index].options = ['', ''];
    
        if (newQuestions[index].type === 'multipleChoice') {
            newQuestions[index].answer = [];
        }
    
        setQuestions(newQuestions);
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
    
        const quiz = {
            name,
            description,
            imageUrl,
            language,
            creatorId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            timeLimitMinutes,
        };
    
        const response = await fetch('http://localhost:5276/api/quizzes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(quiz),
        });
    
        if (response.ok) {
            console.log('Quiz submitted successfully');
        } else {
            console.error('Error submitting quiz');
        }
    };

    const handleOptionChange = (questionIndex, optionIndex, event) => {
        const newQuestions = [...questions]; // creează o copie a întrebărilor existente
        const question = newQuestions[questionIndex]; // obține întrebarea curentă
    
        // setează opțiunea ca fiind valoarea inputului
        question.options[optionIndex] = event.target.value;
    
        setQuestions(newQuestions); // actualizează starea întrebărilor
    };

    const addQuestion = () => {
        const newQuestion = { 
            id: nextId, 
            text: '', 
            type: 'trueFalse', 
            options: ['', ''], 
            answer: [] // inițializează 'answer' ca un array gol
        };
    
        if (newQuestion.type === 'multipleChoice') {
            newQuestion.answer = [];
        }
    
        setQuestions([...questions, newQuestion]);
        setNextId(nextId + 1);
    };

    const removeQuestion = (id) => {
        const updatedQuestions = questions.filter(question => question.id !== id);
        setQuestions(updatedQuestions);
    };

    const handleCheckboxChange = (questionIndex, optionIndex) => {
        const newQuestions = [...questions];
        newQuestions[questionIndex].options[optionIndex] = !newQuestions[questionIndex].options[optionIndex];
        setQuestions(newQuestions);
    };

    const handleOptionTextChange = (questionIndex, optionIndex, event) => {
        const newQuestions = [...questions];
        newQuestions[questionIndex].options[optionIndex] = event.target.value;
        setQuestions(newQuestions);
    };

    const handleAnswerChange = (questionIndex, optionIndex, event) => {
        const newQuestions = [...questions]; // creează o copie a întrebărilor existente
        const question = newQuestions[questionIndex]; // obține întrebarea curentă
    
        if (question.type === 'multipleChoice') {
            // pentru întrebările de tip 'multipleChoice', adaugă sau elimină indexul opțiunii din răspuns în funcție de starea checkbox-ului
            if (event.target.checked) {
                question.answer.push(optionIndex);
            } else {
                const answerIndex = question.answer.indexOf(optionIndex);
                if (answerIndex > -1) {
                    question.answer.splice(answerIndex, 1);
                }
            }
        } else {
            // pentru întrebările de tip 'singleChoice', setează răspunsul ca fiind indexul opțiunii selectate
            question.answer = event.target.checked ? optionIndex : null;
        }
    
        setQuestions(newQuestions); // actualizează starea întrebărilor
    };
    

    const renderInputForQuestionType = (question, index) => {
        switch (question.type) {
            case 'trueFalse':
                return (
                    <div className='flex flex-col gap-1 mt-3'>
                        <label className='text-white'>
                            <input type="radio" value="true" checked={question.options[0] === 'true'} onChange={(e) => handleOptionChange(index, 0, e)} />
                            True
                        </label>
                        <label className='text-white'>
                            <input type="radio" value="false" checked={question.options[0] === 'false'} onChange={(e) => handleOptionChange(index, 0, e)} />
                            False
                        </label>
                    </div>
                );
                case 'singleChoice':
            return (
                <div className='flex flex-col gap-1 mt-3'>
                    {question.options.map((option, optionIndex) => (
                        <div key={optionIndex} className='flex items-center gap-2'>
                            <input 
                                type="radio" 
                                name={`singleChoice${index}`} 
                                checked={question.answer === option} 
                                onChange={(e) => handleAnswerChange(index, option, e)} 
                            />
                            <input 
                                type="text" 
                                value={option} 
                                onChange={(e) => handleOptionChange(index, optionIndex, e)} 
                                className="p-2 border border-gray-300 rounded-md"
                            />
                        </div>
                    ))}
                </div>
            );
            case 'multipleChoice':
                return (
                    <div className='flex flex-col gap-1 mt-3'>
                        {question.options.map((option, optionIndex) => (
                            <div key={optionIndex} className='flex items-center gap-2'>
                                <input 
                                    type="checkbox" 
                                    checked={question.answer.includes(optionIndex)} 
                                    onChange={(e) => handleAnswerChange(index, optionIndex, e)} 
                                />
                                <input 
                                    type="text" 
                                    value={option} 
                                    onChange={(e) => handleOptionChange(index, optionIndex, e)} 
                                    className="p-2 border border-gray-300 rounded-md"
                                />
                            </div>
                        ))}
                    </div>
                );
            case 'writtenAnswer':
                return (
                    <input className="p-2 border border-gray-300 rounded-md mt-2" type="text" value={question.options[0]} onChange={(e) => handleOptionChange(index, 0, e)} />
                );
            default:
                return null;
        }
    };

    return (
        <div>
            <Navbar />
            <form onSubmit={handleSubmit} className='flex flex-col items-center'>
                <h1 className='text-[#DEAE9F] text-4xl font-bold mt-6 mb-6'>Create Quiz</h1>
                <div className="mt-4 flex flex-col mt-5">
                    <label htmlFor="name" className="text-white">Name:</label>
                    <input
                        type="text"
                        id="name"
                        value={name}
                        onChange={handleNameChange}
                        placeholder="Name"
                        className="p-2 border border-gray-300 rounded-md"
                    />
                </div>
                <div className="mt-4 flex flex-col mt-5">
                    <label htmlFor="description" className="text-white">Description:</label>
                    <input
                        type="text"
                        id="description"
                        value={description}
                        onChange={handleDescriptionChange}
                        placeholder="Description"
                        className="p-2 border border-gray-300 rounded-md"
                    />
                </div>
                <div className="mt-4 flex flex-col mt-5">
                    <label htmlFor="imageUrl" className="text-white">Image URL:</label>
                    <input
                        type="text"
                        id="imageUrl"
                        value={imageUrl}
                        onChange={handleImageChange}
                        placeholder="Image URL"
                        className="p-2 border border-gray-300 rounded-md"
                    />
                </div>
                <div className="mt-4 flex flex-col mt-5">
                    <label htmlFor="language" className="text-white">Language:</label>
                    <select id="language" value={language} onChange={handleLanguageChange} className="p-2 border border-gray-300 rounded-md">
                        <option value="Romana">Romana</option>
                        <option value="English">English</option>
                    </select>
                </div>
                <div className="mt-4 flex flex-col mt-5">
                    <label htmlFor="timeLimitMinutes" className="text-white">Time Limit (in minutes):</label>
                    <input
                        type="number"
                        id="timeLimitMinutes"
                        value={timeLimitMinutes}
                        onChange={handleTimeLimitChange}
                        placeholder="Time Limit (in minutes)"
                        className="p-2 border border-gray-300 rounded-md"
                    />
                </div>
                <div className="mt-4">
                    {questions.map((question, index) => (
                        <div key={question.id} className="flex flex-col mt-5">
                            <label htmlFor={`question-${index + 1}`} className="text-white">Question {index + 1}:</label>
                            <input
                                type="text"
                                id={`question-${index + 1}`}
                                value={question.text}
                                onChange={(e) => handleQuestionChange(index, e)}
                                placeholder={`Question ${index + 1}`}
                                className="p-2 border border-gray-300 rounded-md"
                            />
                            <select value={question.type} onChange={(e) => handleQuestionTypeChange(index, e)} className="mt-2 p-2 border border-gray-300 rounded-md">
                                <option value="trueFalse">True/False</option>
                                <option value="singleChoice">Single Choice</option>
                                <option value="multipleChoice">Multiple Choice</option>
                                <option value="writtenAnswer">Written Answer</option>
                            </select>
                            {renderInputForQuestionType(question, index)}
                            <button onClick={() => removeQuestion(question.id)} className="mt-2 p-2 bg-red-500 text-white rounded-md">Remove Question</button>
                        </div>
                    ))}
                </div>
                <div className="mt-4">
                    <button onClick={addQuestion} className="p-2 bg-green-500 text-white rounded-md">Add Question</button>
                </div>
                <button type="submit" className="p-2 bg-blue-500 text-white rounded-md mt-4">Submit</button>
            </form>
        </div>
    );
}

export default QuizCreate;
