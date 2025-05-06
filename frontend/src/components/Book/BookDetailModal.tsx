import { FaCalendar, FaBook, FaHashtag } from 'react-icons/fa';
import { formatCustomDate } from '../../helpers/formatDateHelp';

interface BookDetailsModalProps {
    isOpen: boolean;
    onClose: () => void;
    book: {
        id: string;
        title: string;
        author: string;
        description: string;
        quantity: number;
        available: number;
        publishedDate: string;
        categoryName: string;
    };
}

const BookDetailsModal = ({ isOpen, onClose, book }: BookDetailsModalProps) => {
    if (!isOpen) return null;

    const getStatusColor = () => {
        if (book.available === 0) return 'bg-red-100 text-red-800';
        if (book.available < 3) return 'bg-yellow-100 text-yellow-800';
        return 'bg-green-100 text-green-800';
    };

    const statusText = () => {
        if (book.available === 0) return 'Out of Stock';
        if (book.available < 3) return `Low Stock (${book.available}/${book.quantity})`;
        return `Available (${book.available}/${book.quantity})`;
    };

    return (
        <div className="fixed inset-0 bg-black/20 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl max-h-[90vh] overflow-auto">
            {/* Header */}
            <div className="p-6 border-b border-gray-200">
                <h1 className="text-2xl font-bold text-gray-800">{book.title}</h1>
                <h2 className="text-lg text-gray-600 mt-1">{book.author}</h2>
            </div>

            {/* Main Content */}
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Left Column */}
                <div className="space-y-4">
                <div className="bg-gray-100 w-full h-80 rounded-lg flex items-center justify-center">
                    <span className="text-gray-400">Book Cover</span>
                </div>
                <div className={`px-4 py-2 rounded-full text-sm font-medium text-center ${getStatusColor()}`}>
                    {statusText()}
                </div>
                </div>

                {/* Right Column */}
                <div className="space-y-6">
                {/* Metadata */}
                <div className="space-y-3">
                    <div className="flex items-center text-gray-600">
                    <FaCalendar className="mr-2" />
                    <span>{formatCustomDate(new Date(book.publishedDate))}</span>
                    </div>
                    <div className="flex items-center text-gray-600">
                    <FaBook className="mr-2" />
                    <span>{book.categoryName}</span>
                    </div>
                    <div className="flex items-center text-gray-600">
                    <FaHashtag className="mr-2" />
                    <span className="truncate" title={book.id}>
                        {book.id.slice(0, 8)}...{book.id.slice(-4)}
                    </span>
                    </div>
                </div>

                {/* Description */}
                <div>
                    <h3 className="text-lg font-semibold mb-2">Description</h3>
                    <p className="text-gray-600 text-sm leading-relaxed overflow-y-auto max-h-40">
                    {book.description}
                    </p>
                </div>
                </div>
            </div>

            {/* Footer */}
            <div className="p-6 border-t border-gray-200 flex justify-end space-x-3">
                <button
                onClick={onClose}
                className="px-4 py-2 text-gray-700 hover:bg-gray-50 rounded-lg border border-gray-300"
                >
                Close
                </button>
                <button className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Edit Details
                </button>
            </div>
            </div>
        </div>
    );
};

export default BookDetailsModal;