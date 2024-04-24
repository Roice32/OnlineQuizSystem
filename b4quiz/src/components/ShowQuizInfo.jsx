import React, { useState, useEffect } from "react";

function ShowInfo({ quizID }) {
    const [quizData, setQuizData] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!quizID) return;

        fetch(`https://localhost:7117/api/quizzes/${quizID}`)
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
        <>
            {error ? <p className='text-white'>{error}</p> : null}
            {quizData && !error && (
                <div className="text-white mt-20">
                    {Object.entries(quizData).map(([key, value]) => (
                        <div className='mt-5' key={key}>
                            <div className="font-bold capitalize">
                                {key}:
                            </div> 
                            {key === 'imageUrl' ? (
                                <div>
                                    <img src={value} alt="Quiz Image" style={{ width: '500px', height: '200px', cursor: 'default' }} />
                                </div>
                            ) : (
                                typeof value === 'object' ? <pre>{JSON.stringify(value, null, 2)}</pre> : value
                            )}
                        </div>
                    ))}
                </div>
            )}
        </>
    );
}

export default ShowInfo;
