import React, { useState } from 'react';
import { Book } from '../../models/bookType';
import { formatDate } from '../../helpers/formatDateHelp';
import { FiPlus } from 'react-icons/fi';
import { ImBin } from "react-icons/im";
import { CiWarning } from "react-icons/ci";
import { IoBookOutline, IoClipboardOutline } from 'react-icons/io5';
import { FaCheck, FaRegCalendarAlt, FaUser } from 'react-icons/fa';

interface BookCardProps {
  setIsOpenModal: (isOpen: boolean) => void;
  book: Book;
  isSelected: boolean;
  onBorrowClick: (id: string) => void;
  borrowCount: number;
  monthlyRequestCount: number;
  maxBooksPerRequest: number;
  maxRequestsPerMonth: number;
}

const BookCard: React.FC<BookCardProps> = ({ 
  setIsOpenModal,
  book, 
  isSelected, 
  onBorrowClick, 
  borrowCount, 
  monthlyRequestCount,
  maxBooksPerRequest,
  maxRequestsPerMonth
}) => {
  const [isHovered, setIsHovered] = useState(false);

  const isAvailable = book.available > 0;
  const canBorrowMore = borrowCount < maxBooksPerRequest;
  const canMakeMoreRequests = monthlyRequestCount < maxRequestsPerMonth;

  const getButtonState = () => {
    if (!isAvailable) return 'unavailable';
    if (!canMakeMoreRequests) return 'monthlyLimitReached';
    if (isSelected) return 'return';
    if (!canBorrowMore) return 'requestLimitReached';
    return 'borrow';
  };

  const buttonState = getButtonState();

  const buttonConfigs = {
    unavailable: {
      text: 'Unavailable',
      className: 'bg-gray-300 cursor-not-allowed opacity-70',
      icon: (
        <IoClipboardOutline />

      )
    },
    monthlyLimitReached: {
      text: 'Monthly Limit',
      className: 'bg-purple-300 cursor-not-allowed',
      icon: (
        <IoClipboardOutline />
      )
    },
    requestLimitReached: {
      text: 'Max 5 Books',
      className: 'bg-orange-300 cursor-not-allowed',
      icon: (
        <CiWarning />
      )
    },
    return: {
      text: 'Return',
      className: 'bg-red-500 hover:bg-red-600 shadow-md',
      icon: (
        <ImBin />
      )
    },
    borrow: {
      text: 'Borrow',
      className: 'bg-blue-500 hover:bg-blue-600 shadow-md',
      icon: (
        <FiPlus />
      )
    }
  };

  const currentButton = buttonConfigs[buttonState];
  
  return (
    <div 
      className={`relative max-w-xs rounded-xl overflow-hidden shadow-lg m-4 transition-all duration-300 transform ${isHovered ? '-translate-y-1 shadow-xl' : ''} ${isSelected ? 'bg-blue-50 ring-2 ring-blue-400' : 'bg-white'}`}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <div 
        onClick={setIsOpenModal} 
        className="h-48 w-full bg-gradient-to-br from-blue-100 to-purple-100 flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-blue-300"
      >
        <IoBookOutline className='w-24 h-24 text-gray-400'/>
      </div>
      
      {!isAvailable && (
        <div className="absolute top-4 right-4 bg-red-500 text-white text-xs font-bold px-3 py-1 rounded-full shadow-md">
          OUT OF STOCK
        </div>
      )}
      
      <div className="px-6 py-5">
        <div className="font-bold text-xl mb-2 text-gray-800 line-clamp-2" title={book.title}>
          {book.title}
        </div>
        
        <p className="text-gray-600 text-sm mb-1 flex items-center">
          
          <FaUser className="w-4 h-4 mr-1" />
          {book.author}
        </p>
        
        <p className="text-gray-500 text-xs mb-3 flex items-center">
        <FaRegCalendarAlt className='w-4 h-4 mr-1'/>
          Published: {formatDate(new Date(book.publishedDate),"DD/MM/YYYY")}
        </p>
        
        <div className="mt-4 pt-4 border-t border-gray-100 flex items-center justify-between">
          <div className="flex items-center">
            <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-semibold ${isAvailable ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
              <FaCheck className={`w-4 h-4 mr-1 ${isAvailable ? 'text-green-500' : 'text-gray-500'}`} />
              {isAvailable ? `${book.available} available` : 'Unavailable'}
            </span>
          </div>
          
          <button
            onClick={() => onBorrowClick(book.id)}
            disabled={!isAvailable || (buttonState !== 'borrow' && buttonState !== 'return')}
            className={`px-4 py-2 rounded-lg text-sm font-medium text-white transition-all duration-200 flex items-center ${currentButton.className}`}
          >
            {currentButton.icon}
            {currentButton.text}
          </button>
        </div>
        
        
      </div>
      
    </div>
  );
};

export default BookCard;