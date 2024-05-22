import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import ShowInfo from "../components/ShowQuizInfo";
import Navbar from "../components/Navbar";

function FormGetId() {
    const { id } = useParams();
    const [showInfo, setShowInfo] = useState(false);
    const [quizID, setQuizID] = useState(id || "");
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
        navigate(`/quiz/${quizID}`);
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
        <div className="min-h-screen">
            <Navbar />
            <div className="flex flex-col items-center justify-center py-10">
                <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow-lg w-full max-w-md">
                    <div className="mb-4">
                        <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="quizID">
                            Quiz ID
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
                    <div className="flex items-center justify-between">
                        <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline" type="submit">
                            OK
                        </button>
                    </div>
                </form>

                {showInfo && <div className="mt-10 w-full max-w-md"><ShowInfo quizID={quizID} /></div>}
            </div>
        </div>
    );
}

export default FormGetId;
