import { useState, useContext, FormEvent } from "react";
import { AuthContext } from "../contexts/AuthContext";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";


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