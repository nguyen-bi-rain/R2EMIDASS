import { useEffect, useState } from 'react';
import BookCard from './BookCard';
import bookService from '../../services/book.service';
import Pagination from '../ui/Pagination';
import Spinner from '../ui/Spinner';
import { Book, BookBorrowType } from '../../models/bookType';
import { useAuthContext } from '../../context/authContext';
import { toast } from 'react-toastify';
import BookDetailsModal from './BookDetailModal';

const UserBookList = () => {
    const MAX_BOOKS_PER_REQUEST = 5;
    const MAX_REQUESTS_PER_MONTH = 3;
    const [loading, setLoading] = useState(false);
    const [books, setBooks] = useState<Book[]>([]);
    const [selectedBooks, setSelectedBooks] = useState<string[]>([]);
    const [monthlyRequestCount, setMonthlyRequestCount] = useState(0);
    const [showSubmitModal, setShowSubmitModal] = useState(false);
    const { user } = useAuthContext()
    // Pagination state
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(4);
    const [totalItems, setTotalItems] = useState(0);
    const [isOpenModal, setIsOpenModal] = useState(false);


    useEffect(() => {
        checkMonthlyRequestCount();
        fetchBooks();
    }, [itemsPerPage, currentPage]);

    const checkMonthlyRequestCount = async () => {
        try {
            await bookService.getCurrentMonthRequestCount(user).then((res) => {
                setMonthlyRequestCount(res.data);
            })
        }
        catch (error) {
            console.error("Error fetching monthly request count:", error);
        }
    };

    const fetchBooks = async () => {
        setLoading(true);
        try {
            const res = await bookService.getAllBooks(currentPage, itemsPerPage);
            console.log(res);

            setBooks(res.data.items);
            setItemsPerPage(res.data.pageSize);
            setTotalItems(res.data.totalCount);
        } catch (error) {
            toast.error("Failed to fetch books. Please try again.");
            console.error("Error fetching books:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleBorrowClick = (bookId: string) => {
        setSelectedBooks(prev => 
            prev.includes(bookId) ? prev.filter(id => id !== bookId) : [...prev, bookId]
        );
    };

    const handleSubmitRequest = () => {
        if (selectedBooks.length === 0) return;

        console.log('Submitting request for books:', selectedBooks);
        handleSubmit();

        setBooks(prevBooks =>
            prevBooks.map(book =>
                selectedBooks.includes(book.id)
                    ? { ...book, available: book.available - 1 }
                    : book
            )
        );

        setSelectedBooks([]);
        setShowSubmitModal(false);

    };

    const handleSubmit = async () => {
        const data: BookBorrowType = {
            requestorId: user,
            bookBorrowingRequestDetails: selectedBooks.map((book) => {
                return {
                    bookBorrowingRequestId: 0,
                    bookId: book,
                    createdAt: new Date(),
                }
            })
        }
        await bookService.borrowBook(data).then(() => {
            toast.success("Book borrowing request submitted successfully!");
            const newCount = monthlyRequestCount + 1;
            setMonthlyRequestCount(newCount);
            setSelectedBooks([]);
            setShowSubmitModal(false);
            fetchBooks();
        }).catch((error) => {
            toast.error(error.response?.data?.message || "Failed to submit borrowing request.");
        })
    }

    const selectedBooksDetails = books.filter((book) => selectedBooks.includes(book.id));

    if (loading) {
        return (
            <Spinner />
        )
    }

    return (
        <div className="min-h-screen bg-gray-50 py-8 px-4">
            <div className="max-w-7xl mx-auto">
                <div className="text-center mb-10">
                    <h1 className="text-3xl font-bold text-gray-800 mb-2">Library Books Collection</h1>
                    <p className="text-gray-600">Browse and borrow from our selection of classic literature</p>

                    <div className="mt-4 flex justify-center gap-4">
                        <div className="bg-blue-100 text-blue-800 px-4 py-2 rounded-lg">
                            <span className="font-semibold">Selected:</span> {selectedBooks.length}/{MAX_BOOKS_PER_REQUEST}
                        </div>
                        <div className="bg-purple-100 text-purple-800 px-4 py-2 rounded-lg">
                            <span className="font-semibold">Monthly Requests:</span> {monthlyRequestCount}/{MAX_REQUESTS_PER_MONTH}
                        </div>
                    </div>

                    {selectedBooks.length > 0 && (
                        <button
                            onClick={() => setShowSubmitModal(true)}
                            disabled={monthlyRequestCount >= MAX_REQUESTS_PER_MONTH}
                            className={`mt-4 px-6 py-2 rounded-lg font-medium text-white ${monthlyRequestCount >= MAX_REQUESTS_PER_MONTH
                                ? 'bg-gray-400 cursor-not-allowed'
                                : 'bg-green-500 hover:bg-green-600'
                                }`}
                        >
                            Submit Borrowing Request
                        </button>
                    )}
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                    {books.map((book) => {
                        return < >
                            <BookCard
                                setIsOpenModal={() => setIsOpenModal(true)}
                                key={book.id}
                                book={book}
                                isSelected={selectedBooks.includes(book.id)}
                                onBorrowClick={handleBorrowClick}
                                borrowCount={selectedBooks.length}
                                monthlyRequestCount={monthlyRequestCount}
                                maxBooksPerRequest={MAX_BOOKS_PER_REQUEST}
                                maxRequestsPerMonth={MAX_REQUESTS_PER_MONTH}
                            />
                            <BookDetailsModal
                                book={book}
                                isOpen={isOpenModal}
                                onClose={() => setIsOpenModal(false)}
                            />
                        </>
                    })}
                </div>

                <Pagination
                    currentPage={currentPage}
                    totalPages={Math.ceil(totalItems / itemsPerPage)}
                    onPageChange={setCurrentPage}
                    itemsPerPage={itemsPerPage}
                    onItemsPerPageChange={setItemsPerPage}
                    totalItems={totalItems}
                    showPageInfo
                />

                <div className="mt-8 text-center text-sm text-gray-500">
                    <p>
                        Each book can be borrowed for up to 30 days. Max {MAX_BOOKS_PER_REQUEST} books per request, max{' '}
                        {MAX_REQUESTS_PER_MONTH} requests per month.
                    </p>
                </div>
            </div>

            {showSubmitModal && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
                    <div className="bg-white rounded-xl p-6 max-w-md w-full">
                        <h2 className="text-xl font-bold mb-4">Confirm Borrowing Request</h2>

                        <div className="mb-4">
                            <p className="font-semibold">You are borrowing {selectedBooks.length} book(s):</p>
                            <ul className="list-disc pl-5 mt-2">
                                {selectedBooksDetails.map((book) => (
                                    <li key={book.id} className="text-gray-700">
                                        {book.title}
                                    </li>
                                ))}
                            </ul>
                        </div>

                        <div className="mb-4 bg-yellow-50 p-3 rounded-lg">
                            <p className="text-sm text-yellow-800">
                                Remember: You can only make {MAX_REQUESTS_PER_MONTH - monthlyRequestCount} more request(s) this month.
                            </p>
                        </div>

                        <div className="flex justify-end gap-3">
                            <button
                                onClick={() => setShowSubmitModal(false)}
                                className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleSubmitRequest}
                                className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
                            >
                                Confirm
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UserBookList;