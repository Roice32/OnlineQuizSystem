import Navbar from '../components/Navbar';

const HomePage = () => {
    return (
        <div>
            <Navbar />
            <div className='flex flex-col items-center'>
                <h1 className='text-[#DEAE9F] text-4xl font-bold mt-6'>HomePage</h1>
            </div>
        </div>
    ); 
}

export default HomePage;