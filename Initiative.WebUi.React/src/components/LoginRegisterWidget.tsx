import { useState, useContext, FormEvent } from "react";
import { AuthContext } from "../contexts/AuthContext";
import LoginForm from "./auth/LoginForm";
import RegisterForm from "./auth/RegisterForm";


const LoginRegisterWidget = () => {
 const { isLoggedIn } = useContext(AuthContext); 

 if(isLoggedIn())
 {
    return (<></>);
 }
 else {
    return (<><RegisterForm /> <br/> <LoginForm /></>);    
 }
}

export default LoginRegisterWidget;