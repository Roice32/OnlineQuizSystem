import React, { useState } from 'react';
import Navbar from '../components/Navbar';

const QuizCreate = () => {
    return (
        <div>
            <Navbar />
            <div className='flex flex-col items-center'>
                <h1 className='text-[#DEAE9F] text-4xl font-bold mt-6'>Create Quiz</h1>
            </div>
        </div>
    ); 
}

export default QuizCreate;