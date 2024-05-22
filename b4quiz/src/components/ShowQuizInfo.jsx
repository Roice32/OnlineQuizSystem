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
                    {Object.entries(quizData).map(([key, value]) => (
                        <div className="mt-5" key={key}>
                            <div className="font-bold text-lg capitalize mb-2">
                                {key}:
                            </div>
                            {key === 'imageUrl' ? (
                                <div className="flex justify-center">
                                    <img src={value} alt="Quiz Image" className="rounded-lg shadow-lg w-96 h-48 object-cover" />
                                </div>
                            ) : (
                                typeof value === 'object' ? (
                                    <pre className="bg-gray-700 p-4 rounded-lg">{JSON.stringify(value, null, 2)}</pre>
                                ) : (
                                    <p className="text-base">{value}</p>
                                )
                            )}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default ShowInfo;
