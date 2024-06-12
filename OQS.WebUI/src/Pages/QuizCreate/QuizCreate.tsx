import * as React from "react";
import Navbar from "../../Components/Navbar";
import {
  Dialog,
  TextField,
  Flex,
  CheckboxGroup,
  RadioGroup,
  Select,
  TextArea,
  Tabs,
  Container,
  Box,
  Text,
  Button,
} from "@radix-ui/themes";

import * as Toast from "@radix-ui/react-toast";

import "./QuizCreate.css";

import { useNavigate } from "react-router-dom";

import { useState } from "react";
import { Cross1Icon, PlusIcon, RocketIcon } from "@radix-ui/react-icons";
import { v4 as uuid } from "uuid";
import useAuth from "../../hooks/UseAuth.ts";
import { useSelector } from "react-redux";
import { RootState } from "../../redux/store.ts";

const steps = [
  {
    label: "Quiz details",
  },
];

type QuestionRequest = {
  id: string;
  text: string;
  type: number;
  allocatedPoints: number;
  timeLimit: number;
  quizId?: string;
  trueFalseAnswer?: boolean;
  choices?: string[];
  multipleChoiceAnswers?: string[];
  singleChoiceAnswer?: string;
  writtenAcceptedAnswers?: string[];
};

type Question = {
  inDatabase?: boolean;
  id: string;
  text: string;
  type: number;
  allocatedPoints: number | "";
  timeLimit: number | "";
  trueFalseAnswer?: boolean;
  choices?: string[];
  multipleChoiceAnswers?: string[];
  singleChoiceAnswer?: string;
  writtenAcceptedAnswers?: string[];
};

enum QuestionType {
  TrueFalse = 0,
  MultipleChoice = 1,
  SingleChoice = 2,
  WrittenAnswer = 3,
}

const QuestionTypeString = {
  0: "True/False",
  1: "Multiple Choice",
  2: "Single Choice",
  3: "Written Answer",
};

function optionSingleChoice(value: string, i: number) {
  return (
    <RadioGroup.Item key={i} value={value}>
      <Flex>{value}</Flex>
    </RadioGroup.Item>
  );
}

function optionMultipleChoice(value: string) {
  return (
    <CheckboxGroup.Item value={value}>
      <Flex>{value}</Flex>
    </CheckboxGroup.Item>
  );
}

function QuestionComponent({ id, setQuiz, quiz }) {
  const [addOptionMultipleChoice, setAddOptionMultipleChoice] = useState(false);
  const [newOptionMultipleChoice, setNewOptionMultipleChoice] = useState("");
  const [addOptionWrittenAnswer, setAddOptionWrittenAnswer] = useState(false);
  const [newOptionWrittenAnswer, setNewOptionWrittenAnswer] = useState("");

  const question = quiz.questions.find((q) => q.id === id);

  const questionType = quiz.questions.find((q) => q.id === question.id).type;
  const questionText = quiz.questions.find((q) => q.id === question.id).text;
  const questionAllocatedPoints = quiz.questions.find(
    (q) => q.id === question.id
  ).allocatedPoints;
  const questionTimeLimit = quiz.questions.find(
    (q) => q.id === question.id
  ).timeLimit;

  // const [optionsSingleChoice, setOptionsSingleChoice] = useState<OptionChoice[]>([])
  const [addOptionSingleChoice, setAddOptionSingleChoice] = useState(false);
  const [newOptionSingleChoice, setNewOptionSingleChoice] = useState("");

  const onChangeQuestion = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            text: event.target.value,
          };
        }
        return q;
      }),
    });
  };

  const onChangeQuestionType = (questionTypeValue) => {
    const currentQuestion = quiz.questions.find((q) => q.id === question.id);
    // set the type of current question in quiz
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            type: parseInt(questionTypeValue, 10),
          };
        }
        return q;
      }),
    });
  };

  const renderQuestionTypeOptions = () => {
    switch (questionType) {
      case QuestionType.TrueFalse:
        // eslint-disable-next-line no-case-declarations
        const correctAnswerTrueFalse = quiz.questions
          .find((q) => q.id === question.id)
          .trueFalseAnswer.toString();

        // eslint-disable-next-line no-case-declarations
        const setCorrectAnswerTrueFalse = (value) => {
          setQuiz({
            ...quiz,
            questions: quiz.questions.map((q) => {
              if (q.id === question.id) {
                return {
                  ...q,
                  trueFalseAnswer: value === "true",
                };
              }
              return q;
            }),
          });
        };

        return (
          <>
            <Text as="label">Options</Text>
            <RadioGroup.Root
              value={correctAnswerTrueFalse}
              name="trueFalse"
              onValueChange={setCorrectAnswerTrueFalse}
            >
              <RadioGroup.Item value="true">True</RadioGroup.Item>
              <RadioGroup.Item value="false">False</RadioGroup.Item>
            </RadioGroup.Root>
          </>
        );
      case QuestionType.MultipleChoice:
        // find the current question in the quiz
        // eslint-disable-next-line no-case-declarations
        const currentQuestion = quiz.questions.find(
          (q) => q.id === question.id
        );
        // eslint-disable-next-line no-case-declarations
        const correctAnswers = currentQuestion.multipleChoiceAnswers;

        // eslint-disable-next-line no-case-declarations
        const setCorrectAnswerMultipleChoice = (value) => {
          setQuiz({
            ...quiz,
            questions: quiz.questions.map((q) => {
              if (q.id === question.id) {
                return {
                  ...q,
                  multipleChoiceAnswers: value,
                };
              }
              return q;
            }),
          });
        };

        return (
          <>
            <CheckboxGroup.Root
              value={correctAnswers}
              onValueChange={setCorrectAnswerMultipleChoice}
              name="example"
              mb="10px"
            >
              {question.choices.map((e) => optionMultipleChoice(e))}
            </CheckboxGroup.Root>
            {addOptionMultipleChoice && (
              <>
                <TextArea
                  mb="10px"
                  placeholder="Option"
                  onChange={(event) =>
                    setNewOptionMultipleChoice(event.target.value)
                  }
                />
                <Flex justify="between">
                  <Button
                    mb="20px"
                    onClick={() => {
                      const newOptionsMultipleChoice = [
                        ...question.choices,
                        newOptionMultipleChoice,
                      ];
                      setQuiz({
                        ...quiz,
                        questions: quiz.questions.map((q) => {
                          if (q.id === question.id) {
                            return {
                              ...q,
                              choices: newOptionsMultipleChoice,
                            };
                          }
                          return q;
                        }),
                      });

                      setAddOptionMultipleChoice(false);
                      setNewOptionMultipleChoice("");
                    }}
                  >
                    <PlusIcon /> Add
                  </Button>
                  <Button
                    color="red"
                    mb="20px"
                    onClick={() => {
                      setAddOptionMultipleChoice(false);
                      setNewOptionMultipleChoice("");
                    }}
                  >
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button
              onClick={() => {
                setAddOptionMultipleChoice(true);
              }}
            >
              <PlusIcon /> Add Option
            </Button>
          </>
        );
      case QuestionType.SingleChoice:
        // eslint-disable-next-line no-case-declarations
        const correctAnswerSingleChoice = question.singleChoiceAnswer;
        console.log("DEEE aici", correctAnswerSingleChoice);

        // eslint-disable-next-line no-case-declarations
        const setCorrectAnswerSingleChoice = (value) => {
          setQuiz({
            ...quiz,
            questions: quiz.questions.map((q) => {
              if (q.id === question.id) {
                return {
                  ...q,
                  singleChoiceAnswer: value,
                };
              }
              return q;
            }),
          });
        };

        return (
          <>
            <RadioGroup.Root
              value={correctAnswerSingleChoice}
              onValueChange={setCorrectAnswerSingleChoice}
              name="example"
              mb="10px"
            >
              {question.choices.map((e, i) => optionSingleChoice(e, i))}
            </RadioGroup.Root>
            {addOptionSingleChoice && (
              <>
                <TextArea
                  mb="10px"
                  placeholder="Option"
                  onChange={(event) =>
                    setNewOptionSingleChoice(event.target.value)
                  }
                />
                <Flex justify="between">
                  <Button
                    mb="20px"
                    onClick={() => {
                      const newOptionsSingleChoice = [
                        ...question.choices,
                        newOptionSingleChoice,
                      ];

                      setQuiz({
                        ...quiz,
                        questions: quiz.questions.map((q) => {
                          if (q.id === question.id) {
                            return {
                              ...q,
                              choices: newOptionsSingleChoice,
                            };
                          }
                          return q;
                        }),
                      });

                      setAddOptionSingleChoice(false);
                      setNewOptionSingleChoice("");
                    }}
                  >
                    <PlusIcon /> Add
                  </Button>
                  <Button
                    color="red"
                    mb="20px"
                    onClick={() => {
                      setAddOptionSingleChoice(false);
                      setNewOptionSingleChoice("");
                    }}
                  >
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button
              onClick={() => {
                setAddOptionSingleChoice(true);
              }}
            >
              <PlusIcon /> Add Option
            </Button>
          </>
        );
      case QuestionType.WrittenAnswer:
        return (
          <>
            <Text mb="10px" as="label">
              Answers
            </Text>
            {question.choices && (
              <>
                {question.choices.map((answer, index) => (
                  <Flex direction="column" mb="10px">
                    <Text mb="5px">{answer}</Text>
                    <Button
                      onClick={() => {
                        setQuiz({
                          ...quiz,
                          questions: quiz.questions.map((q) => {
                            if (q.id === question.id) {
                              return {
                                ...q,
                                choices: q.choices.filter((a) => a !== answer),
                              };
                            }
                            return q;
                          }),
                        });
                      }}
                    >
                      <Cross1Icon /> Remove
                    </Button>
                  </Flex>
                ))}
              </>
            )}
            {addOptionWrittenAnswer && (
              <>
                <TextArea
                  mb="10px"
                  placeholder="Option"
                  onChange={(event) =>
                    setNewOptionWrittenAnswer(event.target.value)
                  }
                />
                <Flex justify="between">
                  <Button
                    mb="20px"
                    onClick={() => {
                      const newOptionsWrittenAnswer = [
                        ...question.choices,
                        newOptionWrittenAnswer,
                      ];

                      setQuiz({
                        ...quiz,
                        questions: quiz.questions.map((q) => {
                          if (q.id === question.id) {
                            return {
                              ...q,
                              choices: newOptionsWrittenAnswer,
                              writtenAcceptedAnswers: newOptionsWrittenAnswer,
                            };
                          }
                          return q;
                        }),
                      });

                      setAddOptionWrittenAnswer(false);
                      setNewOptionWrittenAnswer("");
                    }}
                  >
                    <PlusIcon /> Add
                  </Button>
                  <Button
                    color="red"
                    mb="20px"
                    onClick={() => {
                      setAddOptionWrittenAnswer(false);
                      setNewOptionWrittenAnswer("");
                    }}
                  >
                    <Cross1Icon /> Cancel
                  </Button>
                </Flex>
              </>
            )}
            <Button
              onClick={() => {
                setAddOptionWrittenAnswer(true);
              }}
            >
              <PlusIcon /> Add Answer
            </Button>
          </>
        );
    }
  };

  const onChangeAllocatePoints = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            allocatedPoints: parseInt(event.target.value, 10),
          };
        }
        return q;
      }),
    });
  };

  const onChangeAllocateTime = (event) => {
    setQuiz({
      ...quiz,
      questions: quiz.questions.map((q) => {
        if (q.id === question.id) {
          return {
            ...q,
            timeLimit: parseInt(event.target.value, 10),
          };
        }
        return q;
      }),
    });
  };

  return (
    <>
      <Flex direction="column" align="center">
        <Flex direction="column" className="w-10/12 sm:w-96">
          <Box mb="10px">
            <Text as="label">Question</Text>
            <TextArea
              placeholder="Add a question…"
              value={questionText}
              onChange={onChangeQuestion}
            />
          </Box>
          <Box mb="10px">
            <Text as="label">Allocated Points</Text>
            <TextField.Root
              placeholder="Add a allocated points..."
              value={questionAllocatedPoints}
              onChange={onChangeAllocatePoints}
            />
          </Box>
          {/* <Box mb="10px">
                        <Text as="label">Time Limit</Text>
                        <TextField.Root placeholder="Add a time limit..." value={questionTimeLimit}
                                        onChange={onChangeAllocateTime}/>
                    </Box> */}
          <Flex direction="column" mb="10px">
            <Text as="label">Type</Text>
            <Select.Root
              value={QuestionType[questionType]}
              onValueChange={onChangeQuestionType}
            >
              <Select.Trigger>{QuestionType[questionType]}</Select.Trigger>
              <Select.Content>
                {Object.keys(QuestionTypeString).map((key) => (
                  <Select.Item key={key} value={key}>
                    {QuestionTypeString[key]}
                  </Select.Item>
                ))}
              </Select.Content>
            </Select.Root>
          </Flex>
          <Flex direction="column" mb="10px">
            {renderQuestionTypeOptions()}
          </Flex>

          <Button
            mt="20px"
            onClick={() => {
              setQuiz({
                ...quiz,
                questions: quiz.questions.filter((q) => q.id !== question.id),
              });
            }}
          >
            Remove Question
          </Button>
        </Flex>
      </Flex>
    </>
  );
}

type QuizCreateProps = {
  id?: string;
  inDatabase?: boolean;
  name?: string;
  imageUrl?: string;
  description?: string;
  timeLimit?: number | "";
  language?: string;
  questions: Question[];
};

type ErrorMessagesQuizCreate = {
  name?: string;
  imageUrl?: string;
  description?: string;
  timeLimit?: string;
  language?: string;
  questions?: string;
};

const ErrorMessagesQuizCreateString = [
  "name",
  "imageUrl",
  "description",
  "timeLimit",
  "language",
  "questions",
];

export default function QuizCreate() {
  const user = useAuth();
  const userState = useSelector((state: RootState) => state.user);

  const navigate = useNavigate();
  const [quiz, setQuiz] = useState<QuizCreateProps>({
    language: "romanian",
    questions: [],
  });
  const [errorMessages, setErrorMessages] = useState<ErrorMessagesQuizCreate>(
    {}
  );

  const [open, setOpen] = React.useState(false);

  const [messageToast, setMessageToast] = React.useState("");
  const [titleToast, setTitleToast] = React.useState("");

  const [loadingAI, setLoadingAI] = React.useState(false);
  const [doneAI, setDoneAI] = React.useState(false);
  const [prompt, setPrompt] = React.useState("");

  const [editPage, setEditPage] = React.useState(false);

  const [currentElement, setCurrentElement] = React.useState("quiz-details");

  return (
    <>
      <Toast.Provider swipeDirection="right">
        <Dialog.Root>
          <Container p="30px">
            <Tabs.Root value={currentElement} onValueChange={setCurrentElement}>
              <Flex justify="between">
                <Tabs.List size={"2"} wrap={"wrap"}>
                  <Tabs.Trigger value="quiz-details">Quiz details</Tabs.Trigger>
                  {quiz.questions &&
                    quiz.questions.map((question, index) => (
                      <>
                        <Tabs.Trigger value={`question${index}`}>
                          Question {index + 1}
                        </Tabs.Trigger>
                      </>
                    ))}
                  <Tabs.Trigger value="submit-quiz">Submit Quiz</Tabs.Trigger>
                </Tabs.List>

                <Box>
                  <Button
                    mr="10px"
                    onClick={() => {
                      if (!quiz.questions) {
                        setQuiz({
                          ...quiz,
                          questions: [
                            {
                              id: uuid(),
                              text: "",
                              type: 0,
                              allocatedPoints: 0,
                              trueFalseAnswer: true,
                              choices: [],
                              multipleChoiceAnswers: [],
                              writtenAcceptedAnswers: [],
                              timeLimit: 1,
                            },
                          ],
                        });
                      } else {
                        setQuiz({
                          ...quiz,
                          questions: [
                            ...quiz.questions,
                            {
                              id: uuid(),
                              text: "",
                              type: 0,
                              choices: [],
                              trueFalseAnswer: true,
                              multipleChoiceAnswers: [],
                              writtenAcceptedAnswers: [],
                              allocatedPoints: 0,
                              timeLimit: 1,
                            },
                          ],
                        });
                      }
                    }}
                  >
                    Add Question
                  </Button>
                  <Dialog.Trigger>
                    <Button>
                      <RocketIcon />
                    </Button>
                  </Dialog.Trigger>
                </Box>
              </Flex>

              <Box pt="3" width="100%">
                <Tabs.Content value="quiz-details">
                  <Flex direction="column" align="center">
                    <Flex className="w-10/12 sm:w-96" direction="column">
                      <Box mb="10px">
                        <Text as="label">Name</Text>
                        <TextField.Root
                          placeholder="Add a name..."
                          value={quiz.name}
                          onChange={(event) => {
                            if (!event.target.value) {
                              if (event.target.value === "") {
                                setQuiz({
                                  ...quiz,
                                  name: event.target.value,
                                });
                              }
                              setErrorMessages({
                                ...errorMessages,
                                name: "Name is required",
                              });
                            } else {
                              setQuiz({ ...quiz, name: event.target.value });
                              setErrorMessages({ ...errorMessages, name: "" });
                            }
                          }}
                        />
                        {errorMessages.name && (
                          <Text as="p" color="red">
                            {errorMessages.name}
                          </Text>
                        )}
                      </Box>
                      <Box mb="10px">
                        <Text as="label">Image URL</Text>
                        <TextField.Root
                          placeholder="Add a image url..."
                          value={quiz.imageUrl}
                          onChange={(event) => {
                            if (!event.target.value) {
                              if (event.target.value === "") {
                                setQuiz({
                                  ...quiz,
                                  imageUrl: event.target.value,
                                });
                              }
                              setErrorMessages({
                                ...errorMessages,
                                imageUrl: "Image URL is required",
                              });
                            } else {
                              setQuiz({
                                ...quiz,
                                imageUrl: event.target.value,
                              });
                              setErrorMessages({
                                ...errorMessages,
                                imageUrl: "",
                              });
                            }
                          }}
                        />
                        {errorMessages.imageUrl && (
                          <Text as="p" color="red">
                            {errorMessages.imageUrl}
                          </Text>
                        )}
                      </Box>
                      <Box mb="10px">
                        <Text as="label">Description</Text>
                        <TextArea
                          placeholder="Add a description..."
                          value={quiz.description}
                          onChange={(event) => {
                            if (!event.target.value) {
                              if (event.target.value === "") {
                                setQuiz({
                                  ...quiz,
                                  description: event.target.value,
                                });
                              }
                              setErrorMessages({
                                ...errorMessages,
                                description: "Description is required",
                              });
                            } else {
                              setQuiz({
                                ...quiz,
                                description: event.target.value,
                              });
                              setErrorMessages({
                                ...errorMessages,
                                description: "",
                              });
                            }
                          }}
                        />
                        {errorMessages.description && (
                          <Text as="p" color="red">
                            {errorMessages.description}
                          </Text>
                        )}
                      </Box>
                      <Flex mb="10px" direction="column">
                        <Text as="label">Language</Text>
                        <Select.Root
                          defaultValue="romanian"
                          value={quiz.language}
                          onValueChange={(value) => {
                            setQuiz({ ...quiz, language: value });
                          }}
                        >
                          <Select.Trigger />
                          <Select.Content>
                            <Select.Item value="romanian">Romanian</Select.Item>
                            <Select.Item value="english">English</Select.Item>
                            <Select.Item value="spanish">Spanish</Select.Item>
                            <Select.Item value="french">French</Select.Item>
                          </Select.Content>
                        </Select.Root>
                      </Flex>
                      <Box mb="10px">
                        <Text as="label">Time limit</Text>
                        <TextField.Root
                          placeholder="Add a time limit..."
                          value={quiz.timeLimit}
                          onChange={(event) => {
                            // verify if a string is a number in js

                            if (
                              !isNaN(Number(event.target.value)) &&
                              Number(event.target.value) > 0
                            ) {
                              setQuiz({
                                ...quiz,
                                timeLimit: Number(event.target.value),
                              });
                              setErrorMessages({
                                ...errorMessages,
                                timeLimit: "",
                              });
                            } else {
                              if ("" === event.target.value) {
                                setQuiz({
                                  ...quiz,
                                  timeLimit: event.target.value,
                                });
                              }
                              setErrorMessages({
                                ...errorMessages,
                                timeLimit: "Invalid time limit",
                              });
                            }
                          }}
                        />
                        {errorMessages.timeLimit && (
                          <Text as="p" color="red">
                            {errorMessages.timeLimit}
                          </Text>
                        )}
                      </Box>
                    </Flex>
                  </Flex>
                </Tabs.Content>

                {quiz.questions &&
                  quiz.questions.map((question, index) => (
                    <>
                      <Tabs.Content value={`question${index}`}>
                        <QuestionComponent
                          id={question.id}
                          setQuiz={setQuiz}
                          quiz={quiz}
                        />
                      </Tabs.Content>
                    </>
                  ))}

                <Tabs.Content value="submit-quiz">
                  <Flex justify="center">
                    <Flex direction="column" justify="center" maxWidth="500px">
                      <Text mb="20px">
                        If you are ready you can submit your quiz
                      </Text>
                      <Button
                        onClick={async () => {
                          const quizRequest = {
                            name: quiz.name,
                            imageUrl: quiz.imageUrl,
                            timeLimitMinutes: quiz.timeLimit,
                            language: quiz.language,
                            description: quiz.description,
                            creatorId:
                              user?.id ||
                              "822c13a0-8872-431e-ad64-00f5249db11f",
                          };

                          try {
                            let response: Response;
                            const token = userState.user?.token;
                            if (editPage) {
                              response = await fetch(
                                `http://localhost:5276/api/quizzes/${quiz.id}`,
                                {
                                  method: "PATCH",
                                  headers: {
                                    "Content-Type": "application/json",
                                    Authorization: `Bearer ${token}`,
                                  },
                                  body: JSON.stringify(quizRequest),
                                }
                              );
                            } else {
                              response = await fetch(
                                "http://localhost:5276/api/quizzes",
                                {
                                  method: "POST",
                                  headers: {
                                    "Content-Type": "application/json",
                                    Authorization: `Bearer ${token}`,
                                  },
                                  body: JSON.stringify(quizRequest),
                                }
                              );
                            }

                            const data = await response.json();
                            let quizId: string | undefined = undefined;

                            if (typeof data === "string") {
                              setEditPage(true);
                              quizId = data.substring(13, data.length);
                            }

                            if (response.status === 200) {
                              setEditPage(true);
                              setQuiz({
                                ...quiz,
                                id: quizId || quiz.id,
                              });
                              quizId = quizId || quiz.id;

                              for (const question of quiz.questions) {
                                const indexQuestion =
                                  quiz.questions.indexOf(question);

                                console.log("Questiiooonn", question);

                                let questionRequest: QuestionRequest = {
                                  id: question.id,
                                  text: question.text,
                                  type: question.type,
                                  allocatedPoints:
                                    question.allocatedPoints || 0,
                                  timeLimit: question.timeLimit || 0,
                                  quizId: quizId,
                                };

                                if (question.type == QuestionType.TrueFalse) {
                                  questionRequest = {
                                    ...questionRequest,
                                    trueFalseAnswer: question.trueFalseAnswer,
                                  };
                                } else if (
                                  question.type == QuestionType.MultipleChoice
                                ) {
                                  questionRequest = {
                                    ...questionRequest,
                                    choices: question.choices,
                                    multipleChoiceAnswers:
                                      question.multipleChoiceAnswers,
                                  };
                                } else if (
                                  question.type == QuestionType.SingleChoice
                                ) {
                                  questionRequest = {
                                    ...questionRequest,
                                    choices: question.choices,
                                    singleChoiceAnswer:
                                      question.singleChoiceAnswer,
                                  };
                                } else if (
                                  question.type == QuestionType.WrittenAnswer
                                ) {
                                  questionRequest = {
                                    ...questionRequest,
                                    choices: question.choices,
                                    writtenAcceptedAnswers:
                                      question.writtenAcceptedAnswers,
                                  };
                                }

                                let response: Response;
                                let data;
                                let questionId = question.id;

                                if (question.inDatabase) {
                                  const token = userState.user?.token;
                                  response = await fetch(
                                    `http://localhost:5276/api/quizzes/${quizId}/questions/${questionId}`,
                                    {
                                      method: "PATCH",
                                      headers: {
                                        "Content-Type": "application/json",
                                        Authorization: `Bearer ${token}`,
                                      },
                                      body: JSON.stringify(questionRequest),
                                    }
                                  );
                                  data = await response.json();
                                } else {
                                  const token = userState.user?.token;
                                  response = await fetch(
                                    `http://localhost:5276/api/quizzes/${quizId}/questions`,
                                    {
                                      method: "POST",
                                      headers: {
                                        "Content-Type": "application/json",
                                        Authorization: `Bearer ${token}`,
                                      },
                                      body: JSON.stringify(questionRequest),
                                    }
                                  );
                                  data = await response.json();
                                  questionId = data;
                                }

                                if (response.status !== 200) {
                                  setOpen(true);
                                  setMessageToast(data.message);
                                  setTitleToast(
                                    `Error at the question ${indexQuestion + 1}`
                                  );
                                  console.log(data.message);
                                  return;
                                }

                                setQuiz({
                                  ...quiz,
                                  id: quizId,
                                  questions: quiz.questions.map((q) => {
                                    if (q.id === question.id) {
                                      return {
                                        ...q,
                                        inDatabase: true,
                                        id: questionId,
                                      };
                                    }
                                    return q;
                                  }),
                                });
                              }

                              console.log("de aici", data);
                              if (typeof data === "string") {
                                setEditPage(true);
                                return navigate(data.substring(4, data.length));
                              } else if (data.id) {
                                return navigate("/quizzes/" + data.id);
                              }
                            }

                            console.log(data);
                            if (data.code !== 200) {
                              setOpen(true);
                              setMessageToast(data.message);
                              setTitleToast("Error at creating the quiz");
                              console.log(data.message);
                              const errorMessagesStrings =
                                data?.message.split("\n");
                              let errorMessages = {};
                              for (const error of errorMessagesStrings) {
                                if (error.toLowerCase().includes("name")) {
                                  errorMessages = {
                                    ...errorMessages,
                                    name: error,
                                  };
                                }
                                if (error.toLowerCase().includes("image")) {
                                  errorMessages = {
                                    ...errorMessages,
                                    imageUrl: error,
                                  };
                                }
                                if (
                                  error.toLowerCase().includes("description")
                                ) {
                                  errorMessages = {
                                    ...errorMessages,
                                    description: error,
                                  };
                                }
                                if (error.toLowerCase().includes("time")) {
                                  errorMessages = {
                                    ...errorMessages,
                                    timeLimit: error,
                                  };
                                }
                                if (error.toLowerCase().includes("language")) {
                                  errorMessages = {
                                    ...errorMessages,
                                    language: error,
                                  };
                                }
                              }

                              setErrorMessages({ ...errorMessages });
                            }

                            console.log("Success:", data);
                          } catch (error) {
                            if (error instanceof SyntaxError) {
                              setOpen(true);
                              setMessageToast(
                                "Review the quiz details and the questions!"
                              );
                              setTitleToast("Error at creating the quiz");
                            } else {
                              console.error("Error:", error);
                            }
                          }
                        }}
                      >
                        Submit Quiz
                      </Button>
                    </Flex>
                  </Flex>
                </Tabs.Content>
              </Box>
            </Tabs.Root>
          </Container>
          <Toast.Root className="ToastRoot" open={open} onOpenChange={setOpen}>
            <Toast.Title className="ToastTitle">{titleToast}</Toast.Title>
            <Toast.Description asChild>
              <Box className="ToastDescription">{messageToast}</Box>
            </Toast.Description>
            <Toast.Action className="ToastAction" asChild altText="Confirm">
              <Button>Confirm</Button>
            </Toast.Action>
          </Toast.Root>
          <Toast.Viewport className="ToastViewport" />

          <Dialog.Content maxWidth="450px">
            <Dialog.Title>Generate question</Dialog.Title>
            <Dialog.Description size="2" mb="4">
              Enter your prompt for AI to generate a question
            </Dialog.Description>

            <Flex direction="column" gap="3">
              <label>
                <Text as="div" size="2" mb="1" weight="bold">
                  Prompt
                </Text>
                <TextField.Root
                  value={prompt}
                  onChange={(event) => setPrompt(event.target.value)}
                  placeholder="Enter your prompt"
                />
              </label>
            </Flex>

            <Flex gap="3" mt="4" justify="end">
              {!loadingAI && (
                <Dialog.Close>
                  <Button variant="soft" color="gray">
                    Cancel
                  </Button>
                </Dialog.Close>
              )}

              {!doneAI && !loadingAI && (
                <Button
                  onClick={async () => {
                    setLoadingAI(true);

                    const currentElementNumber = currentElement.substring(8);
                    // verify if current element is number
                    if (isNaN(Number(currentElementNumber))) {
                      setLoadingAI(false);
                      setMessageToast("You need to be on a question");
                      setTitleToast("Error");
                      setOpen(true);
                      return;
                    }

                    const mesaj = `Vreau să creezi 1 întrebări de dificultate medie, fiecare având 4 variante de răspuns, una corectă și 3 greșite, pentru un quiz cu tema "${prompt}". Vei returna raspunsul sub forma de json, unde intrebarea va fi un string, iar variantele de raspuns vor fi un array de stringuri. Varianta corecta va fi prima varianta posibila.
                Exemplu: {
                    "1": {
                        "intrebare": "Cine a fost primul rege al Angliei?",
                        "variante": [
                            "William I Cuceritorul",
                            "Henry VIII",
                            "Elizabeth I",
                            "Richard III"
                        ]
                    }
                }`;

                    const response = await fetch(
                      "https://api.textcortex.com/v1/codes",
                      {
                        method: "POST",
                        headers: {
                          Authorization:
                            "Bearer gAAAAABmOiEDhlUAHKGx2bE9D5STmMvKgsuM2FaNLHVZ3_OWSGEJvhsPCfyztsWqT9V-03iE-uoHVSoRVZgdTeW593DH7j2Uc1ZLBIe_ySogrTb91Sjq72zVyzZc6KwBXzvBdTJ19AwZ",
                          "Content-Type": "application/json",
                        },
                        body: JSON.stringify({
                          max_tokens: 2048,
                          mode: "python",
                          model: "icortex-1",
                          n: 1,
                          temperature: 0,
                          text: mesaj,
                        }),
                      }
                    );

                    if (!response.ok) {
                      setOpen(true);
                      setMessageToast("");
                      setTitleToast("Service currently unavailable!");
                      setDoneAI(false);
                      setLoadingAI(false);
                    } else {
                      const responseData = await response.json();
                      console.log("Răspunsul API:", responseData);
                      const raspunsAI = responseData.data.outputs[0].text;
                      const responseObject = JSON.parse(raspunsAI);

                      const correctAnswer = responseObject["1"].variante[0];
                      const text = responseObject["1"].intrebare;
                      const choices = responseObject["1"].variante;
                      console.log(choices);

                      const currentQuestion = quiz.questions?.find(
                        (q) =>
                          q.id ===
                          quiz.questions[Number(currentElementNumber)].id
                      ) || { id: 3 };

                      setQuiz({
                        ...quiz,
                        questions: quiz.questions?.map((q) => {
                          if (q.id === currentQuestion?.id) {
                            return {
                              ...q,
                              text: text,
                              type: 2,
                              choices: choices,
                              singleChoiceAnswer: correctAnswer,
                            };
                          }
                          return q;
                        }),
                      });
                      setDoneAI(true);
                      setLoadingAI(false);
                    }
                  }}
                >
                  Generate question
                </Button>
              )}

              {loadingAI && <Button loading>Generate question</Button>}

              {doneAI && (
                <Dialog.Close>
                  <Button
                    onClick={() => {
                      setDoneAI(false);
                    }}
                  >
                    Done
                  </Button>
                </Dialog.Close>
              )}
            </Flex>
          </Dialog.Content>
        </Dialog.Root>
      </Toast.Provider>
    </>
  );
}
