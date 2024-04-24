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
        <div>
            <Navbar />
            <div className="flex items-center flex-col ">

                <form onSubmit={handleSubmit} >
                    <div className="flex flex-col items-center justify-center">
                        <label>
                            <input
                                type="text"
                                value={quizID}
                                onChange={handleChange} 
                                onKeyDown={handleKeyDown} 
                            />
                        </label>
                        <span>
                            <button className="mt-5 text-white border rounded-3 rounded w-10" type="submit">OK</button>
                        </span>
                    </div> 
                </form>

                {showInfo && <ShowInfo quizID={quizID} />}
            </div>
        </div>
    );
}

export default FormGetId;
