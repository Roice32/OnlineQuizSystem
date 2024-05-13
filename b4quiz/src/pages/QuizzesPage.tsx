import {useEffect, useState} from "react";

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
    console.log(props.quizzes)
    return (
        <ul className="list-none space-y-4">
            {props.quizzes.map((quiz) => (
                <li key={quiz.id} className="p-4 bg-blue-200 rounded shadow">
                    {quiz.name}
                </li>
            ))}
        </ul>
    );
}

function Pagination(props: {
    offset: number,
    onChangeOffset: (value: (((prevState: number) => number) | number)) => void,
    limit: number,
    onChangeLimit: (value: (((prevState: number) => number) | number)) => void
}) {
    return (
        <div className="flex justify-center items-center space-x-4 mt-8">
            <button
                onClick={() => props.onChangeOffset((prev) => prev - 1)}
                className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
            >
                Previous
            </button>
            <button
                onClick={() => props.onChangeOffset((prev) => prev + 1)}
                className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
            >
                Next
            </button>
            <select
                value={props.limit}
                onChange={(e) => props.onChangeLimit(Number(e.target.value))}
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
        fetch(`http://localhost:5276/api/quizzes?limit=${limit}&offset=${offset}`)
            .then((response) => response.json() as Promise<QuizResponse>)
            .then((data) => setData(data))
            .catch((error) => setError(error))
            .finally(() => setIsLoading(false));
    }, [limit, offset]);

    return {data, error, isLoading};
}

const QuizzesPage = () => {
    const [limit, setLimit] = useState(10);
    const [offset, setOffset] = useState(0);
    const {data, error, isLoading} = useQuizzes(limit, offset);

    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;
    if (!data) return <div>No data</div>;

    return (
        <div className="flex flex-col items-center bg-gray-100 p-4 rounded-lg shadow-lg">
            <h1 className="text-2xl font-bold mb-4">Quizzes</h1>
            <QuizzesList quizzes={data.quizzes}/>
            <Pagination
                offset={data.pagination.offset}
                limit={limit}
                onChangeLimit={setLimit}
                onChangeOffset={setOffset}
            />
        </div>
    );
}

export default QuizzesPage;
