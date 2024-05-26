import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
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

function QuizzesList(props: { quizzes: Quiz[], onDelete: (id: string) => void }) {
    return (
        <div className="space-y-4">
            {props.quizzes.map((quiz) => (
                <div key={quiz.id} className="flex items-center">
                    <li className="flex-1 p-4 bg-[#EEEFEE] rounded shadow list-none">
                        {quiz.name}
                    </li>
                    <Link to={`http://localhost:3000/create-quiz`}>
                        <div className="w-8 h-8 bg-blue-500 text-white flex items-center justify-center ml-4 rounded">
                            <FontAwesomeIcon icon={faEdit} />
                        </div>
                    </Link>
                    <div
                        className="w-8 h-8 bg-red-500 text-white flex items-center justify-center ml-2 rounded cursor-pointer"
                        onClick={() => props.onDelete(quiz.id)}
                    >
                        <FontAwesomeIcon icon={faTrashAlt} />
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

    const handleDelete = (id: string) => {
        fetch(`http://localhost:5276/api/quizzes/${id}`, {
            method: 'DELETE'
        })
            .then(response => {
                if (response.ok) {
                    // If delete was successful, refetch the quizzes
                    reloadQuizzes();
                    // Optionally reset to the first page
                    setOffset(0);
                } else {
                    // Handle error case
                    console.error('Failed to delete quiz');
                }
            })
            .catch(error => {
                console.error('Error deleting quiz:', error);
            });
    };

    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;
    if (!data) return <div>No data</div>;

    return (
        <div className="flex flex-col items-center">
            <Navbar />
            <div className="flex flex-col items-center bg-[#E6DEDA] p-4 rounded-lg shadow-lg max-w-[500px] mt-11">
                <h1 className="text-4xl font-bold mb-4 text-[#376060]">Quizzes</h1>
                <QuizzesList quizzes={data.quizzes} onDelete={handleDelete} />
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
