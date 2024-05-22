import React, { useState } from 'react';
import Navbar from '../components/Navbar';

const HomePage = () => {
  const [faqs, setFaqs] = useState([
    { question: 'How do I create a quiz?', answer: 'To create a quiz, click on the "Create a Quiz" button.' },
    { question: 'Can I edit an existing quiz?', answer: 'Yes, you can edit any existing quiz from your dashboard.' },
  ]);

  const [newQuestion, setNewQuestion] = useState('');
  const [newAnswer, setNewAnswer] = useState('');

  const handleAddFaq = () => {
    if (newQuestion.trim() && newAnswer.trim()) {
      setFaqs([...faqs, { question: newQuestion, answer: newAnswer }]);
      setNewQuestion('');
      setNewAnswer('');
    }
  };

  const handleDeleteFaq = (indexToDelete) => {
    setFaqs(faqs.filter((_, index) => index !== indexToDelete));
  };

  return (
    <div>
      <Navbar />
      <div className="flex flex-col items-center">
        <h1 className="text-[#1c4e4f] text-4xl font-bold mt-6">B4Quiz</h1>
      </div>
      <div className="flex flex-wrap mt-10 px-6">
        <div className="w-full md:w-1/2 px-4">
          <h2 className="text-2xl text-[#1c4e4f] font-semibold mb-4">User Reviews</h2>
          <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 mb-4 rounded-md shadow-md">
            <p>⭐⭐⭐⭐⭐</p>
            <p>“This platform is fantastic! Easy to use and very efficient.” - Maria</p>
          </div>
          <div className="bg-[#efd7cf] bg-opacity-50 p-4 mb-4 rounded-md shadow-md">
            <p>⭐⭐⭐⭐</p>
            <p>“I really enjoyed using this platform for my quizzes.” - John</p>
          </div>
        </div>
        <div className="w-full md:w-1/2 px-4">
          <h2 className="text-2xl text-[#1c4e4f] font-semibold mb-4">Frequently Asked Questions (FAQ)</h2>
          <div className="space-y-4 mb-6">
            {faqs.map((faq, index) => (
              <div key={index} className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md relative">
                <h3 className="font-semibold">{faq.question}</h3>
                <p>{faq.answer}</p>
                <button
                  onClick={() => handleDeleteFaq(index)}
                  className="absolute top-2 right-2 text-[#a49e97] hover:text-[#1c4e4f]"
                >
                  Delete
                </button>
              </div>
            ))}
          </div>
          <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
            <input
              type="text"
              placeholder="New Question"
              value={newQuestion}
              onChange={(e) => setNewQuestion(e.target.value)}
              className="w-full p-2 mb-2 border border-gray-300 rounded-md bg-[#f7ebe7] bg-opacity-50"
            />
            <input
              type="text"
              placeholder="New Answer"
              value={newAnswer}
              onChange={(e) => setNewAnswer(e.target.value)}
              className="w-full p-2 mb-2 border border-gray-300 rounded-md bg-[#f7ebe7] bg-opacity-50"
            />
            <button
              onClick={handleAddFaq}
              className="w-full p-2 bg-[#436e6f] text-[#f7ebe7] rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]"
            >
              Add
            </button>
          </div>
        </div>
      </div>

      <div className="flex justify-start px-6 mt-10">
        <h2 className="text-2xl text-[#1c4e4f] font-semibold">Take a quiz</h2>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 mt-10 px-6">
        <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz1.jpg" alt="Quiz 1" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Math Quiz</h3>
            <p>Test your math skills with this challenging quiz.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#efd7cf] text-[#1c4e4f] px-4 py-2 rounded-md hover:bg-[#1c4e4f] hover:text-[#f7ebe7]">
                Take Quiz
              </button>
              <p>10 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz2.jpg" alt="Quiz 2" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Geography Quiz</h3>
            <p>Explore your knowledge of the world with this geography quiz.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#436e6f] text-[#f7ebe7] px-4 py-2 rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]">
                Take Quiz
              </button>
              <p>19 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz3.jpg" alt="Quiz 1" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">History Quiz</h3>
            <p>This quiz covers a wide range of topics in world history, from ancient civilizations to modern events. It tests your understanding of historical figures, significant events, and the evolution of societies across the globe.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#efd7cf] text-[#1c4e4f] px-4 py-2 rounded-md hover:bg-[#1c4e4f] hover:text-[#f7ebe7]">
                Take Quiz
              </button>
              <p>10 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz4.jpg" alt="Quiz 2" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Music Quiz: Tune in to Your Musical Knowledge</h3>
            <p>This quiz covers a wide range of musical genres, artists, and historical periods. It challenges you to identify songs, instruments, musical terms, and the influence of music on various cultures and societies.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#436e6f] text-[#f7ebe7] px-4 py-2 rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]">
                Take Quiz
              </button>
              <p>15 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz5.jpg" alt="Quiz 2" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Sports Quiz: Prove Your Athletic Expertise</h3>
            <p>This quiz tests your knowledge of different sports, teams, athletes, and sporting events. It covers a variety of sports, from popular team games to individual disciplines, and aims to test your understanding of sports history, rules, and accomplishments.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#436e6f] text-[#f7ebe7] px-4 py-2 rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]">
                Take Quiz
              </button>
              <p>17 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz6.jpg" alt="Quiz 1" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Literature Quiz: Bookworm's Delight</h3>
            <p>This quiz focuses on literature, testing your familiarity with classic and contemporary novels, authors, literary movements, and literary devices. It's designed to engage avid readers and literature enthusiasts.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#efd7cf] text-[#1c4e4f] px-4 py-2 rounded-md hover:bg-[#1c4e4f] hover:text-[#f7ebe7]">
                Take Quiz
              </button>
              <p>8 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz7.jpg" alt="Quiz 2" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Computer Science Quiz: Byte-Sized Challenge</h3>
            <p>This quiz delves into the fundamental concepts of computer science, including data structures, algorithms, programming languages, computer hardware, and software development principles. Test your problem-solving skills and technical knowledge.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#436e6f] text-[#f7ebe7] px-4 py-2 rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]">
                Take Quiz
              </button>
              <p>15 Questions</p>
            </div>
          </div>
        </div>
        <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md">
          <img src="quiz8.jpg" alt="Quiz 1" className="w-full h-32 object-cover rounded-t-md" />
          <div className="p-2">
            <h3 className="font-semibold">Psychology Quiz: Unravel the Mind</h3>
            <p>Explore the fascinating field of psychology with this quiz. It covers a range of topics, including human behavior, cognitive processes, mental disorders, developmental stages, and prominent psychological theories and experiments.</p>
            <div className="flex justify-between mt-2">
              <button className="bg-[#efd7cf] text-[#1c4e4f] px-4 py-2 rounded-md hover:bg-[#1c4e4f] hover:text-[#f7ebe7]">
                Take Quiz
              </button>
              <p>12 Questions</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
