import { useEffect, useState } from "react";
import Navbar from "../Components/Navbar";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faEdit } from '@fortawesome/free-solid-svg-icons';
import { Link } from 'react-router-dom';

type Quiz = {
    id: string;
    name: string;
    description: string;
    language: string | null;
    imageUrl: string | null;
    timeLimitMinutes: number;
    createdAt: string;
    questions: any[];
};

type Pagination = {
    offset: number;
    limit: number;
    totalRecords: number;
};

type QuizResponse = {
    pagination: Pagination;
    quizzes: Quiz[];
};
function QuizzesList(props: { quizzes: Quiz[] }) {
    const defaultImageUrl = 'https://www.shutterstock.com/shutterstock/photos/2052894734/display_1500/stock-vector-quiz-and-question-marks-trivia-night-quiz-symbol-neon-sign-night-online-game-with-questions-2052894734.jpg';

    return (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 mt-10 w-full px-6">
            {props.quizzes.map((quiz, index) => (
                <div key={quiz.id} className={`relative p-4 rounded-md shadow-md bg-opacity-50 ${index % 2 === 0 ? 'bg-[#436e6f] text-[#f7ebe7]' : 'bg-[#deae9f]'}`}>
                    <img 
                        src={quiz.imageUrl || defaultImageUrl} 
                        alt={quiz.name} 
                        onError={(e) => e.currentTarget.src = defaultImageUrl} 
                        className="w-full h-32 object-cover rounded-t-md" 
                    />
                    <div className="p-2">
                        <h3 className="font-semibold">{quiz.name}</h3>
                        <p className="mb-4">{quiz.description}</p>
                    </div>
                    <div className="absolute bottom-2 right-2">
                        <Link to={`/quizzes/${quiz.id}`}>
                            <button
                                className={`py-2 px-4 rounded-full bg-[#436e6f] text-white uppercase text-lg font-bold shadow-md transition duration-300 hover:bg-[#efd7cf] hover:text-[#0a2d2e] hover:border-[#0a2d2e] `}
                            >
                                Play
                            </button>
                        </Link>
                    </div>
                </div>
            ))}
        </div>
    );
}







function Pagination(props: {
    offset: number;
    limit: number;
    totalRecords: number;
    onChangeOffset: (value: (((prevState: number) => number) | number)) => void,
    onChangeLimit: (value: (((prevState: number) => number) | number)) => void
}) {
    const { offset, limit, totalRecords, onChangeOffset, onChangeLimit } = props;

    const totalPages = Math.ceil(totalRecords / limit);

    return (
        <div className="flex justify-center items-center space-x-4 mt-8">
            <button
                onClick={() => onChangeOffset(Math.max(offset - limit, 0))}
                disabled={offset === 0}
                className="px-4 py-2 bg-[#436e6f] text-[#E6DEDA] rounded hover:bg-[#1c4e4f]"
            >
                Previous
            </button>
            <span>Page {Math.floor(offset / limit) + 1} of {totalPages}</span>
            <button
                onClick={() => onChangeOffset(offset + limit)}
                disabled={offset + limit >= totalRecords}
                className="px-4 py-2 bg-[#436e6f] text-[#E6DEDA] rounded hover:bg-[#1c4e4f]"
            >
                Next
            </button>
            <select
                value={limit}
                onChange={(e) => {
                    onChangeLimit(Number(e.target.value));
                    onChangeOffset(0);
                }}
                className="px-2 py-1 border rounded"
            >
                <option value={5}>5</option>
                <option value={10}>10</option>
                <option value={15}>15</option>
            </select>
        </div>
    );
}

function useQuizzes(limit: number, offset: number) {
    const [data, setData] = useState<QuizResponse | undefined>();
    const [error, setError] = useState<Error | null>();
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        fetch(`http://localhost:5276/api/quizzes?offset=${offset}&limit=${limit}`)
            .then((response) => response.json() as Promise<QuizResponse>)
            .then((data) => setData(data))
            .catch((error) => setError(error))
            .finally(() => setIsLoading(false));
    }, [limit, offset]);

    const reloadQuizzes = () => {
        setIsLoading(true);
        fetch(`http://localhost:5276/api/quizzes?offset=${offset}&limit=${limit}`)
            .then((response) => response.json() as Promise<QuizResponse>)
            .then((data) => setData(data))
            .catch((error) => setError(error))
            .finally(() => setIsLoading(false));
    };

    return { data, error, isLoading, reloadQuizzes };
}

const QuizzesPage = () => {
    const [limit, setLimit] = useState(10);
    const [offset, setOffset] = useState(0);
    const { data, error, isLoading, reloadQuizzes } = useQuizzes(limit, offset);

   

    if (isLoading) return <div className="col-span-full flex justify-center mt-10"><div className="spinner"></div></div>;
    if (error) return <div>Error: {error.message}</div>;
    if (!data) return <div>No data</div>;

    return (
        <div className="flex flex-col items-center w-full">
            <Navbar />
            <div className="flex flex-col items-center bg-[#E6DEDA] p-4 rounded-lg shadow-lg w-full mt-11">
                <h1 className="text-4xl font-bold mb-4 text-[#376060]">Quizzes</h1>
                <QuizzesList quizzes={data.quizzes} />
                <Pagination
                    offset={data.pagination.offset}
                    limit={data.pagination.limit}
                    totalRecords={data.pagination.totalRecords}
                    onChangeLimit={setLimit}
                    onChangeOffset={setOffset}
                />
            </div>
        </div>
    );
}

export default QuizzesPage;
