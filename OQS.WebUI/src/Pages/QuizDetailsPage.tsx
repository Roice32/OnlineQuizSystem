import axios from "../utils/axios-service";
import {Quiz} from "../utils/types/quiz";
import {Result} from "../utils/types/result";
import {
    LoaderFunctionArgs,
    useLoaderData,
    useNavigate, useParams,
} from "react-router-dom";
import ShowQuizInfo from "../Components/ShowQuizInfo";

interface PageProps {
    params: { id: string };
}

export const QuizDetailsLoader = async ({
                                            params: {id},
                                        }: LoaderFunctionArgs<PageProps>): Promise<Quiz> => {
    const response = (await axios.get(`/api/quizzes/${id}`, {}))
        .data as Result<Quiz>;
    if (response.isFailure) {
        throw new Error(response.error?.message);
    }
    return response.value as Quiz;
};

type CreateActiveQuizRequest = {
    quizId: string;
};

type CreateLiveQuizRequest = {
    quizId: string;
    userId: string;
};

export default function QuizDetailsPage() {
    const {id} = useParams();

    return (
        <div>
            <ShowQuizInfo quizId={id}/>
        </div>
    );
}
