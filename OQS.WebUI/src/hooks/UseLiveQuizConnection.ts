import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { ConnectionRequest } from "../utils/types/connection-request";
import { useLocation, useNavigate } from "react-router-dom";
import { userMock } from "../utils/mocks/userMock";
import { useDispatch, useSelector } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import { Error } from "../utils/types/error";
import { config } from "../config";
import { Result } from "../utils/types/result";
import { RootState } from "../redux/store";
import { setConnection as setConnectionState,disconnect } from "../redux/Connection/ConnectionState";


export function UseLiveQuizConnectionMock(){

}



export default function UseLiveQuizConnection() {
    const [connection,setConnection]=useState<HubConnection|null>(null);
    const [isAdmin, setIsAdmin] = useState<boolean>(false);
    const [connectedUsers, setConnectedUsers] = useState<string[]>([]);
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useDispatch();
    const userState=useSelector((state:RootState)=>state.user);
    const code = location.pathname.split("/")[2];
    const connectionRequest: ConnectionRequest = {
        userId: userState.user?.id as string,
        code,
    };
    console.log("connectionRequest",connectionRequest);

    const startQuiz = async () => {
        if(config.useBackend)
        {await connection?.invoke("StartQuiz");}
    };

    const exitQuiz = async () => {
        if(config.useBackend){
            
            await connection?.stop();
            setConnection(null);
            dispatch(disconnect());
            navigate("/quizzes");
        }
    };

    useEffect(() => {
        const connect=async()=>{
            try {
          const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5276/ws/live-quizzes")
            .build();
            
            connection.on("Error", (error:Error) => {
                dispatch(openSnackbar({message:error.message,severity:"error"}));
                if(error.code==='JoinRoom.BadRequest'){
                    navigate("/quizzes");
                }
            });

            connection.on("Joined", (senderIsAdmin) => {
                setIsAdmin(senderIsAdmin);
                setConnection(connection);
                dispatch(setConnectionState(connection));
            });
            connection.on("UserJoined", (name) => {
                setConnectedUsers((prev) => [...prev, name]);
                console.log("User joined", name);
            });
            connection.on("QuizStarted",async (res:Result<string>) => {
                if(res.isSuccess){
                   
                        await connection.stop();
                        navigate(`/active-quizzes/${res.value}`);
                        dispatch(disconnect());
                        setConnection(null);
                        
                     dispatch(openSnackbar({message:"The quiz has started",severity:"success"}));
                }else{
                    dispatch(openSnackbar({message:res.error?.message as string,severity:"error"}));
                }
            });
            connection.on("QuizStartedAdmin",async (res:Result<string>) => {
                if(res.isSuccess){
                   
                        await connection.stop();
                   
                        navigate(`/quizzes/${res.value}`);
                        setConnection(null);
                        dispatch(disconnect());
                     dispatch(openSnackbar({message:"The quiz has started",severity:"success"}));
                }else{
                    dispatch(openSnackbar({message:res.error?.message as string,severity:"error"}));
                }
            });
            connection.on("UserLeft",(name)=>{
                setConnectedUsers((prev) => [...prev.filter((user) => user !== name)]);
                dispatch(openSnackbar({message:`${name} has left the quiz`,severity:"info"}));
                
            });
            connection.on("QuizCanceled",async ()=>{
                dispatch(openSnackbar({message:"The quiz has been canceled",severity:"info"}));
                await connection.stop();
                setConnection(null);
                dispatch(disconnect());
                navigate("/quizzes");
            });
            await connection.start();
            connection.invoke("JoinQuiz", connectionRequest);
            dispatch(openSnackbar({message:"Successfully joined live quiz",severity:"success"}));
          } catch (error) {
            dispatch(openSnackbar({message:"Failed to join live quiz",severity:"error"}));
          }
        }
        if(config.useBackend){
            connect();
        }
                   return  () => {
                dispatch(disconnect());
            };
        
    }, []);
    if(config.useBackend){
        return {connection,isAdmin,connectedUsers,startQuiz,exitQuiz};
    }

    return {connection,isAdmin:true,connectedUsers:["user1","user2","user3","user4","user5","user6","user7","user8","user9","user10"],startQuiz,exitQuiz};

}