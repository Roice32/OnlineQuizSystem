import { useDispatch } from "react-redux";
import { QuestionDisplayProps } from "./QuestionDisplay";
import { submitAnswer } from "../../redux/ActiveQuiz/ActiveQuizzesState";

export default function WrittenQuestionDisplay({
  question,
  selectedAnswer,
  activeQuizId,
}: QuestionDisplayProps) {
  const dispatch = useDispatch();
  const handleChange = (e) => {
    const writeAnswer = e.target.value;
    const answer = {
      questionId: question.id,
      questionType: question.type,
      writeAnswer,
    };
    dispatch(
      submitAnswer({
        activeQuizId,
        answer,
      })
    );
  };

  return (
    <div className="flex flex-col items-center p-4">
      <div className="w-full max-w-4xl bg-white rounded-lg p-6 shadow-lg">
        <h2 className="text-2xl font-bold mb-4 text-center">{question.text}</h2>
        <div className="flex items-center">
          <div className="w-1/3 p-4">
            <img src="/src/utils/mocks/question-mark.png" alt="Question" className="w-full h-auto rounded-lg" />
          </div>
          <div className="w-2/3 p-4">
            <textarea
              onBlur={handleChange}
              className="w-full p-2 border border-gray-300 rounded-lg resize-none focus:outline-none focus:ring-2 focus:ring-[#deae9f] "
              rows={4} 
              defaultValue={selectedAnswer?.writeAnswer}
            />
          </div>
        </div>
      </div>
    </div>
  );
}
