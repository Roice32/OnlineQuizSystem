import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ShowInfo from "../components/ShowQuizInfo";
import Navbar from "../components/Navbar";

function FormGetId() {
    const { id } = useParams();
    const [quizID, setQuizID] = useState(id || "");
    const [showInfo, setShowInfo] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        if (id) {
            setQuizID(id);
            setShowInfo(true);
        }
    }, [id]);

    const handleSubmit = (e) => {
        e.preventDefault();
        setShowInfo(true);
        navigate(`/quizzes/${quizID}`);
    };

    const handleKeyDown = (e) => {
        if (e.key === 'Enter') {
            handleSubmit(e);
        }
    };

    const handleChange = (e) => {
        setQuizID(e.target.value);
        setShowInfo(false);
    };

    return (
        <div className="min-h-screen bg-gray-100">
            <Navbar />
            <div className="flex flex-col items-center justify-center py-10">
                <h2 className="text-3xl font-bold text-blue-600 mb-6">Search Quiz</h2>
                <form onSubmit={handleSubmit} className="bg-white p-8 rounded-lg shadow-lg w-full max-w-md">
                    <div className="mb-4 text-center">
                        <label className="block text-gray-700 text-lg font-bold mb-2" htmlFor="quizID">
                            Quiz ID:
                        </label>
                        <input
                            id="quizID"
                            type="text"
                            value={quizID}
                            onChange={handleChange}
                            onKeyDown={handleKeyDown}
                            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                        />
                    </div>
                    <div className="flex items-center justify-center">
                        <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit">
                            Search
                        </button>
                    </div>
                </form>


            </div>
        </div>


    );
}

export default FormGetId;
