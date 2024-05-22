import React, { useState, useEffect } from "react";

function ShowInfo({ quizID }) {
    const [quizData, setQuizData] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!quizID) return;

        fetch(`http://localhost:5276/api/quizzes/${quizID}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Quiz ID not found");
                }
                return response.json();
            })
            .then(data => {
                console.log("Quiz data:", data);
                setQuizData(data);
                setError(null);
            })
            .catch(error => {
                console.error("Error fetching quiz info:", error);
                setError("Quiz ID not found");
            });
    }, [quizID]);

    return (
        <div className="container mx-auto p-6">
            {error ? (
                <p className="text-red-500 text-center">{error}</p>
            ) : null}
            {quizData && !error && (
                <div className="bg-gray-800 p-8 rounded-lg shadow-md text-white mt-10">
                    <div className="mt-5">
                        <div className="font-bold text-lg capitalize mb-2">
                            Description:
                        </div>
                        <p className="text-base">{quizData.description}</p>
                    </div>
                </div>
            )}
        </div>
    );
}

export default ShowInfo;
