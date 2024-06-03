import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../Components/Navbar";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrashAlt, faEdit } from '@fortawesome/free-solid-svg-icons';
import { useParams } from 'react-router-dom';
//import ModifyQuizPage from "./ModifyQuizPage";
//<Route path="/:id/modify" component={ModifyQuizPage} />
type Quiz = {
    id: string;
    name: string;
    description: string;
    language: string | null;
    creatorId: string;
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

type QuizzesListProps = {
    quizzes: Quiz[];
    onDelete: (id: string) => void;
    onModify: (id: string) => void;
};

const QuizzesList: React.FC<QuizzesListProps> = ({ quizzes, onDelete, onModify }) => {
    const sortedQuizzes = [...quizzes].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    return (
        <ul className="list-none w-full max-w-2xl">
            {sortedQuizzes.map((quiz) => (
                <li key={quiz.id} className="flex flex-col py-2">
                    <div className="w-full border-t border-b border-[#efd7cf] border-solid border-2">
                        <div className="flex flex-col py-2 px-4">
                            <span className="text-[#1c4e4f] text-2xl font-bold">{quiz.name}</span>
                            <div className="flex items-center">
                                <p className="ml-4 text-sm text-[#1c4e4f]">Created at: {new Date(quiz.createdAt).toLocaleDateString()}</p>
                                <div className="ml-auto">
                                    <button 
                                        onClick={() => onModify(quiz.id)} 
                                        className="bg-[#436e6f] text-white px-4 py-2 rounded mr-2"
                                    >
                                        <FontAwesomeIcon icon={faEdit} />
                                    </button>
                                    <button 
                                        onClick={() => onDelete(quiz.id)} 
                                        className="bg-[#efd7cf] text-[#1c4e4f] px-4 py-2 rounded"
                                    >
                                        <FontAwesomeIcon icon={faTrashAlt} />
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </li>
            ))}
        </ul>
    );
};

type PaginationProps = {
    offset: number;
    limit: number;
    totalRecords: number;
    onChangeOffset: (value: number) => void;
    onChangeLimit: (value: number) => void;
};

const Pagination: React.FC<PaginationProps> = ({ offset, limit, totalRecords, onChangeOffset, onChangeLimit }) => {
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
};

function useQuizzes(userId: string, limit: number, offset: number) {
    const [data, setData] = useState<QuizResponse | undefined>();
    const [error, setError] = useState<Error | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const fetchQuizzes = () => {
        setIsLoading(true);
        fetch(`http://localhost:5276/api/quizzes?offset=${offset}&limit=${limit}`)
            .then((response) => response.json() as Promise<QuizResponse>)
            .then((data) => {
                // Filtrarea quizurilor după creatorId
                const filteredQuizzes = data.quizzes.filter((quiz) => quiz.creatorId === userId);
                // Actualizarea stării doar cu quizurile filtrate
                setData({ ...data, quizzes: filteredQuizzes });
            })
            .catch((error) => setError(error))
            .finally(() => setIsLoading(false));
    };

    useEffect(() => {
        fetchQuizzes();
    }, [userId, limit, offset]);

    return { data, error, isLoading, reloadQuizzes: fetchQuizzes };
}

const MyQuizzesPage: React.FC = () => {
    //const userId = "00000000-0000-0000-0001-000000000002"; // Înlocuiește acest ID cu ID-ul real al utilizatorului
    const { userId } = useParams<{ userId: string }>();
    const [limit, setLimit] = useState<number>(10);
    const [offset, setOffset] = useState<number>(0);
    const { data, error, isLoading, reloadQuizzes } = useQuizzes(userId || "00000000-0000-0000-0001-000000000002", limit, offset);
    const navigate= useNavigate();

    const handleDelete = (id: string) => {
        fetch(`http://localhost:5276/api/quizzes/${id}`, {
            method: 'DELETE'
        })
            .then(response => {
                if (response.ok) {
                    reloadQuizzes();
                    setOffset(0);
                } else {
                    console.error('Failed to delete quiz');
                }
            })
            .catch(error => {
                console.error('Error deleting quiz:', error);
                alert('Failed to delete quiz. Please try again later.');
            });
    };

    const handleModify = (id: string) => {
        navigate("/&{id}/modify");
    };

    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;
    if (!data) return <div>No data</div>;

    return (
        <div>
            <Navbar />
            <div className="flex flex-col items-center">
                <h1 className="text-[#1c4e4f] text-4xl font-bold mt-6">My Quizzes</h1>
                <div className="mt-6 w-full max-w-2xl">
                    <QuizzesList quizzes={data.quizzes} onDelete={handleDelete} onModify={handleModify} />
                    <Pagination
                        offset={data.pagination.offset}
                        limit={data.pagination.limit}
                        totalRecords={data.pagination.totalRecords}
                        onChangeLimit={setLimit}
                        onChangeOffset={setOffset}
                    />
                </div>
            </div>
        </div>
    );
};

export default MyQuizzesPage;
