import { useRef, useState, useEffect } from "react";

const LoginForm = () => {
  const userRef = useRef();
  const errRef = useRef();

  const [user, setUser] = useState("");
  const [password, setPassword] = useState("");
  const [errorMsg, setErrorMsg] = useState("");
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    userRef.current.focus();
  }, []);

  useEffect(() => {
    setErrorMsg("");
  }, [user, password]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log(user, password);
    setUser("");
    setPassword("");
    setSuccess(true);
  };

  return (
    <>
      {success ? (
        <h1>Inloggad</h1>
      ) : (
        <div className="h-auto flex flex-col border p-4 rounded-xl px-10 py-20 bg-white border-gray">
          <p
            ref={errRef}
            className={errorMsg ? "errorMsg" : "offscreen"}
            aria-live="assertive"
          >
            {errorMsg}
          </p>
          <div>
            <h1 className="text-3xl text-center font-bold mb-5">Sign in!</h1>
            <p className="font-medium mb-8 text-center">
              Use local account to sign in.
            </p>
          </div>

          <form className="flex flex-col bg-white" onSubmit={handleSubmit}>
            <div>
              <div className="text-center">
                <label htmlFor="email">Email</label>
                <input
                  className="w-full border-2 border-gray rounded-xl p-3 mt-2 mb-4 bg-transparent"
                  type="text"
                  id="email"
                  placeholder="Email"
                  ref={userRef}
                  autoComplete="off"
                  onChange={(e) => setUser(e.target.value)}
                  value={user}
                  required
                />
              </div>
            </div>

            <div>
              <div className="text-center">
                <label htmlFor="password">Password</label>
                <input
                  className="w-full border-2 border-gray rounded-xl p-3 mt-2 bg-transparent"
                  type="password"
                  id="password"
                  placeholder="Password"
                  onChange={(e) => setPassword(e.target.value)}
                  value={password}
                  required
                />
              </div>
            </div>

            <div className="mt-5 flex justify-between items-center">
              <div className="flex gap-1">
                <input
                  type="checkbox"
                  id="checkbox"
                  className="accent-medium-green checked rounded-xl ml-1"
                />
                <label className="ml-2 font-medium text-base" htmlFor="checkbox">
                  Remember me
                </label>
              </div>
            </div>

            <div className="text-center mt-6 mb-6 flex">
              <button
                className=" w-full rounded-xl m-2 hover:scale-[1.01] ease-in-out text-lg text-green px-4 
              py-2 shadow-md shadow-brown bg-medium-green text-white transition-all duration-200 active:scale-[.98]"
              >
                Sign In
              </button>
            </div>
          </form>
          <span>Forgot your password? Contact us by email.</span>
        </div>
      )}
    </>
  );
};

export default LoginForm;
