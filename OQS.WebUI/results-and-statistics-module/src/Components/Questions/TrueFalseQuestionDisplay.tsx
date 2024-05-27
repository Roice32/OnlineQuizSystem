/* eslint-disable @typescript-eslint/no-unused-vars */
import { useDispatch } from "react-redux";
import { QuestionDisplayProps } from "./QuestionDisplay";
import { Answer } from "../../utils/types/active-quiz";
import { submitAnswer } from "../../redux/ActiveQuiz/ActiveQuizzesState";

export default function TrueFalseQuestionDisplay({
  question,
  activeQuizId,
  selectedAnswer,
}: QuestionDisplayProps) {
  const dispatch = useDispatch();
  const handleChange = (e) => {
    let trueFalseAnswer: boolean | undefined = true;
    if (e.target.id === "true") {
      trueFalseAnswer = selectedAnswer?.trueFalseAnswer !== true ? true : undefined;
    } else {
      trueFalseAnswer = selectedAnswer?.trueFalseAnswer !== false ? false : undefined;
    }
    const answer: Answer = {
      questionId: question.id,
      questionType: question.type,
      trueFalseAnswer,
    };
    dispatch(submitAnswer({ activeQuizId, answer }));
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
            <div className="space-y-4">
              <div className="flex items-center justify-center mb-2">
                <input
                  type="checkbox"
                  id="true"
                  checked={selectedAnswer?.trueFalseAnswer === true}
                  onChange={handleChange}
                  className="hidden"
                />
                <label
                  htmlFor="true"
                  className={`cursor-pointer p-2 rounded-full border-2 border-black-300 w-full max-w-md text-left ${
                    selectedAnswer?.trueFalseAnswer === true
                      ? "bg-[#1c4e4f] text-white"
                      : "bg-white text-black"
                  } hover:bg-[#deae9f] hover:text-white`}
                >
                  True
                </label>
              </div>
              <div className="flex items-center justify-center mb-2">
                <input
                  type="checkbox"
                  id="false"
                  checked={selectedAnswer?.trueFalseAnswer === false}
                  onChange={handleChange}
                  className="hidden"
                />
                <label
                  htmlFor="false"
                  className={`cursor-pointer p-2 rounded-full border border-gray-300 w-full max-w-md text-left ${
                    selectedAnswer?.trueFalseAnswer === false
                      ? "bg-[#1c4e4f] text-white"
                      : "bg-white text-black"
                  } hover:bg-[#deae9f] hover:text-white`}
                >
                  False
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
