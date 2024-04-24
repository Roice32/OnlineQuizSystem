import React, { useState } from 'react';
import Navbar from '../components/Navbar';

const QuizCreate = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [image, setImage] = useState('');
    const [language, setLanguage] = useState('Romana');
    const [timeLimit, setTimeLimit] = useState(0);
    const [questions, setQuestions] = useState([{ id: 1, text: '' }]);
    const [nextId, setNextId] = useState(2);

    const handleNameChange = (event) => {
        setName(event.target.value);
    };

    const handleDescriptionChange = (event) => {
        setDescription(event.target.value);
    };

    const handleImageChange = (event) => {
        setImage(event.target.value);
    };

    const handleLanguageChange = (event) => {
        setLanguage(event.target.value);
    };

    const handleTimeLimitChange = (event) => {
        setTimeLimit(parseInt(event.target.value));
    };

    const handleQuestionChange = (index, event) => {
        const newQuestions = [...questions];
        newQuestions[index].text = event.target.value;
        setQuestions(newQuestions);
    };

    const addQuestion = () => {
        setQuestions([...questions, { id: nextId, text: '' }]);
        setNextId(nextId + 1);
    };

    const removeQuestion = (id) => {
        const updatedQuestions = questions.filter(question => question.id !== id);
        setQuestions(updatedQuestions);
    };

    return (
        <div>
            <Navbar />
            <div className='flex flex-col items-center'>
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
                    <label htmlFor="image" className="text-white">Image URL:</label>
                    <input
                        type="text"
                        id="image"
                        value={image}
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
                    <label htmlFor="timeLimit" className="text-white">Time Limit (in minutes):</label>
                    <input
                        type="number"
                        id="timeLimit"
                        value={timeLimit}
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
                            <button onClick={() => removeQuestion(question.id)} className="mt-2 p-2 bg-red-500 text-white rounded-md">Remove Question</button>
                        </div>
                    ))}
                </div>
                <div className="mt-4">
                    <button onClick={addQuestion} className="p-2 bg-green-500 text-white rounded-md">Add Question</button>
                </div>
            </div>
        </div>
    );
}

export default QuizCreate;
