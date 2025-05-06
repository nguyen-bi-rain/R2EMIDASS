import { useState, useEffect } from 'react';
import { NAVIGATION_ITEMS } from '../../constants';
import { Link } from 'react-router-dom';
import { useAuthContext } from '../../context/authContext';

const Navbar = () => {
    const { role } = useAuthContext();
    const [navigationItems, setNavigationItems] = useState(NAVIGATION_ITEMS(role));
    const [isCollapsed, setIsCollapsed] = useState(false);

    useEffect(() => {
        const activeIndex = localStorage.getItem('activeNavIndex');
        if (activeIndex !== null) {
            const updatedItems = navigationItems.map((item, i) => ({
                ...item,
                isActive: i === parseInt(activeIndex, 10),
            }));
            setNavigationItems(updatedItems);
        }

        // Auto-collapse on smaller screens
        const handleResize = () => {
            setIsCollapsed(window.innerWidth < 768);
        };

        handleResize(); // Set initial state
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    const handleItemClick = (index: number) => {
        const updatedItems = navigationItems.map((item, i) => ({
            ...item,
            isActive: i === index,
        }));
        setNavigationItems(updatedItems);
        localStorage.setItem('activeNavIndex', index.toString());
    };

    const toggleCollapse = () => {
        setIsCollapsed(!isCollapsed);
    };

    return (
        <div 
            className={`h-full bg-indigo-800 text-white shadow-lg transition-all duration-300 ${
                isCollapsed ? 'w-20' : 'w-64'
            }`}
        >
            <div className="p-4 flex items-center justify-between border-b border-indigo-700">
                {!isCollapsed ? (
                    <div className="flex items-center space-x-3">
                        <i className="fas fa-book-open text-2xl text-indigo-300"></i>
                        <h1 className="text-xl font-bold">Libra<span className="text-indigo-300">Sys</span></h1>
                    </div>
                ) : (
                    <div className="flex justify-center w-full">
                        <i className="fas fa-book-open text-2xl text-indigo-300"></i>
                    </div>
                )}
                <button 
                    onClick={toggleCollapse}
                    className="text-indigo-300 hover:text-white ml-2"
                    aria-label={isCollapsed ? "Expand sidebar" : "Collapse sidebar"}
                >
                    {isCollapsed ? (
                        <i className="fas fa-chevron-right"></i>
                    ) : (
                        <i className="fas fa-chevron-left"></i>
                    )}
                </button>
            </div>
            <nav className="p-4 space-y-2">
                {navigationItems.map((item, i) => (
                    <Link
                        key={item.name + i}
                        to={`${item.path}`}
                        className={`flex items-center space-x-3 p-3 rounded-lg ${
                            item.isActive ? "bg-indigo-700" : "hover:bg-indigo-700"
                        } text-indigo-200 hover:text-white`}
                        onClick={() => handleItemClick(i)}
                        title={isCollapsed ? item.name : ''}
                    >
                        <span className="flex items-center">
                            {item.icon}
                            {!isCollapsed && <span className='ml-3'>{item.name}</span>}
                        </span>
                    </Link>
                ))}
            </nav>
        </div>
    );
};

export default Navbar;