import React from "react";
import { useParams } from "react-router-dom";
import Navbar from "../components/Navbar";
import ShowInfo from "../components/ShowQuizInfo";

function QuizPage() {
    const { quizID } = useParams();
    
    return (
        <div className="min-h-screen">
            <Navbar />
                  
            <ShowInfo quizID={quizID} />
        </div>
    );
}

export default QuizPage;
