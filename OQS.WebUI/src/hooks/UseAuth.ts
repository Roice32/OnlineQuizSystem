import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import { config } from "../config";

export default function useAuth(){
    const userState = useSelector((state: RootState) => state.user);
    const navigate=useNavigate();
    const dispatch = useDispatch();
    useEffect(() => {
    if(!userState.isLogged){
        if(config.useBackend){
        dispatch(openSnackbar({message: "You need to login first", severity: "error"}));
        navigate('/auth/login');
        }
    }
    }, [userState.isLogged, navigate]);
    return userState.user;
}