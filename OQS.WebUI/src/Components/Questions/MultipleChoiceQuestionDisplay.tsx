import { useDispatch } from "react-redux";
import { QuestionDisplayProps } from "./QuestionDisplay";
import { submitAnswer } from "../../redux/ActiveQuiz/ActiveQuizzesState";
import { Answer } from "../../utils/types/active-quiz";

export default function MultipleChoiceQuestionDisplay({
  question,
  activeQuizId,
  selectedAnswer,
}: QuestionDisplayProps) {
  const dispatch = useDispatch();

  const handleChange = (e) => {
    let multipleChoiceAnswers = selectedAnswer?.multipleChoiceAnswers ?? [];

    if (multipleChoiceAnswers.includes(e.target.value)) {
      multipleChoiceAnswers = multipleChoiceAnswers.filter((answer) => answer !== e.target.value);
    } else {
      multipleChoiceAnswers = [...multipleChoiceAnswers, e.target.value];
    }

    const answer: Answer = {
      questionId: question.id,
      questionType: question.type,
      multipleChoiceAnswers,
    };
    dispatch(
      submitAnswer({
        activeQuizId,
        answer,
      })
    );
  };

  return (
    <div className="flex flex-col items-center p-4 font-mono">
      <div className="w-full max-w-4xl bg-white rounded-lg p-6 shadow-lg">
        <h2 className="text-2xl font-bold mb-4 text-center">{question.text}</h2>
        <div className="flex items-center">
          <div className="w-1/3 p-4">
            <img src="/src/utils/mocks/question-mark.png" alt="Question" className="w-full h-auto rounded-lg" />
          </div>
          <div className="w-2/3 p-4">
            <div className="space-y-4">
              {question.choices?.map((choice, i) => (
                <div key={i} className="flex items-center justify-center mb-2">
                  <input
                    type="checkbox"
                    id={choice}
                    name={question.id}
                    value={choice}
                    checked={selectedAnswer?.multipleChoiceAnswers?.includes(choice)}
                    onChange={handleChange}
                    className="hidden"
                  />
                  <label
                    htmlFor={choice}
                    className={`cursor-pointer p-2 rounded-full border border-gray-300 w-full max-w-md text-left ${
                      selectedAnswer?.multipleChoiceAnswers?.includes(choice)
                        ? "bg-[#1c4e4f] text-white"
                        : "bg-white text-black"
                    } hover:bg-[#deae9f] hover:text-white`}
                  >
                    {choice}
                  </label>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
