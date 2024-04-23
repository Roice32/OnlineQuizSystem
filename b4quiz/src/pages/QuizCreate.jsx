import React, { useState } from 'react';
import Navbar from '../components/Navbar';

const QuizCreate = () => {
    const [name, setName] = useState('');
    const [description, setDescription] = useState('');
    const [image, setImage] = useState('');
    const [language, setLanguage] = useState('Romana'); 
    const [timeLimit, setTimeLimit] = useState(0);

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
            </div>
        </div>
    ); 
}

export default QuizCreate;