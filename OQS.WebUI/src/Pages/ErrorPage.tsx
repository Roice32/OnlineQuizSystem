import { useRouteError } from "react-router-dom";

export default function ErrorPage() {
  const err = useRouteError() as Error;
  console.log(err);
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="bg-white rounded-lg shadow-lg p-8 max-w-lg text-center">
        <h1 className="text-4xl font-bold text-red-600 mb-4">Oops!</h1>
        <p className="text-xl text-gray-800 mb-4">Something went wrong.</p>
        <p className="text-gray-600 mb-8">{err?.message}</p>
        <a href="/" className="bg-white text-[#1c4e4f] px-4 py-2 rounded-full transition-transform transform hover:bg-[#deae9f] hover:scale-105">
          Go to Home
        </a>
      </div>
    </div>
  );
}
