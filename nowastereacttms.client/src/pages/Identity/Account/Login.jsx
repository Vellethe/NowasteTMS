import React from 'react'
import LoginForm from '../../../components/LoginForm';
import Nowastebg from '..//..//..//assets/Images/Nowastebg.jpg';



const Login = () => {
  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gradient-to-l from-dark-green via-medium-green to-white ">
      {/* <img
        className="w-full h-screen bg-cover bg-center bg-no-repeat bg-fixed relative "
        src={Nowastebg}
      /> */}
      <div className="absolute">
        <LoginForm />
      </div>
    </div>
  );
}

export default Login
