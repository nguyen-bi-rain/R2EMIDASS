import { useEffect, useState } from 'react';
import { FaBook, FaUser, FaCalendarCheck, FaCheckCircle, FaChevronDown, FaChevronUp, FaCheck } from 'react-icons/fa';
import Button from '../../components/ui/Button';
import { BookBorrowResponseType } from '../../models/bookType';
import { IoClose } from "react-icons/io5";
import { useAuthContext } from '../../context/authContext';
import bookService from '../../services/book.service';
import { toast } from 'react-toastify';
import Spinner from '../../components/ui/Spinner';
import Pagination from '../../components/ui/Pagination'; // Import the Pagination component

const BorrowingRequests = () => {
  const [expandedRequest, setExpandedRequest] = useState(null);
  const [status, setStatus] = useState(-1);
  const [requests, setRequests] = useState<BookBorrowResponseType[]>([]);
  const [loading, setLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10); // Default items per page
  const { user, role } = useAuthContext();

  useEffect(() => {
    fetchRequests();
  }, [status, currentPage, itemsPerPage]);

  const fetchRequests = async () => {
    setLoading(true);
    try {
      let res;
      if (role === "SUPER_USER") {
        res = await bookService.getAllBorrowedBooksRequest(currentPage, itemsPerPage, status);
      } else {
        res = await bookService.getAllBorrowedBooksRequestByUserId(user, currentPage, itemsPerPage, status);
      }
      setRequests(res.data.items);
      setTotalPages(res.data.totalPages); // Assuming the API returns total pages
    } catch (error) {
      setRequests([]);
      toast.error("Failed to fetch requests. Please try again.");
      console.error("Error fetching requests:", error);
    } finally {
      setLoading(false);
    }
  };

  const toggleRequest = (requestId) => {
    setExpandedRequest(expandedRequest === requestId ? null : requestId);
  };

  const handleStatusRequest = async (status: number, id: number) => {
    const data = {
      id: id,
      status: status,
      approverId: user,
    };
    setLoading(true);
    try {
      await bookService.updateBorrowedBookRequest(data).then(() => {
        fetchRequests();
      });
    } catch (error) {
      console.error("Error updating request:", error);
      toast.error("Error updating request");
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
  };

  const handleItemsPerPageChange = (newItemsPerPage: number) => {
    setItemsPerPage(newItemsPerPage);
    setCurrentPage(1); // Reset to the first page when items per page changes
  };

  if (loading) {
    return <Spinner />;
  }

  return (
    <div className="p-6 max-w-7xl mx-auto">
      {/* Header Section */}
      <div className="mb-8">
        <h1 className="text-2xl font-bold flex items-center mb-4">
          <FaBook className="mr-2 text-blue-500" />
          Borrowing Requests
        </h1>
        <p className="text-gray-600">Manage book borrowing requests from users</p>
      </div>

      {/* Status Tabs */}
      <div className="flex flex-wrap gap-4 mb-6">
        <button
          className={`px-4 py-2 rounded-full flex items-center ${status === -1 ? "bg-blue-100 text-blue-700" : "bg-gray-100 text-gray-700"}`}
          onClick={() => setStatus(-1)}
        >
          <span>All Requests</span>
        </button>
        <button
          className={`px-4 py-2 rounded-full flex items-center ${status === 2 ? "bg-yellow-100 text-yellow-700" : "bg-gray-100 text-gray-700"}`}
          onClick={() => setStatus(2)}
        >
          <span>Waiting</span>
        </button>
        <button
          className={`px-4 py-2 rounded-full flex items-center ${status === 0 ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-700"}`}
          onClick={() => setStatus(0)}
        >
          <span>Approved</span>
        </button>
        <button
          className={`px-4 py-2 rounded-full flex items-center ${status === 1 ? "bg-red-100 text-red-700" : "bg-gray-100 text-gray-700"}`}
          onClick={() => setStatus(1)}
        >
          <span>Rejected</span>
        </button>
      </div>

      {/* Requests List */}
      <div className="space-y-4">
        {requests.map(request => (
          <div key={request.id} className="bg-white rounded-lg shadow-md border border-gray-200 overflow-hidden">
            {/* Request Header - Always visible */}
            <div className="p-4 flex justify-between items-center cursor-pointer hover:bg-gray-50">
              <div onClick={() => toggleRequest(request.id)}>
                <h3 className="font-semibold flex items-center">
                  <FaUser className="mr-2 text-gray-500" />
                  Request #{request.id} by {request.requestName}
                  <span className="ml-2 text-sm text-gray-500">{new Date(request.dateRequest).toLocaleDateString()}</span>
                </h3>
                <div className="mt-1 mb-2 text-sm text-gray-500">
                  Reference: {request.id} | {request.bookBorrowingRequestDetails.length} book{request.bookBorrowingRequestDetails.length > 1 ? 's' : ''}
                </div>
                <span
                  className={`px-3 py-1 rounded-full text-sm mr-3 ${
                    request.status === 2
                      ? "bg-yellow-100 text-yellow-700"
                      : request.status === 0
                      ? "bg-green-100 text-green-700"
                      : "bg-red-100 text-red-700"
                  }`}
                >
                  {request.status === 2 ? "Waiting" : request.status === 0 ? "Approved" : "Rejected"}
                </span>
              </div>
              <div className="flex items-center">
                {request.status === 2 && role === "SUPER_USER" && (
                  <div>
                    <Button variant="primary" size="md" className="mr-2" onClick={() => handleStatusRequest(0, request.id)}>
                      <FaCheck />
                      Approve
                    </Button>
                    <Button variant="danger" size="md" className="mr-2" onClick={() => handleStatusRequest(1, request.id)}>
                      <IoClose /> Reject
                    </Button>
                  </div>
                )}
                <div className="cursor-pointer" onClick={() => toggleRequest(request.id)}>
                  {expandedRequest === request.id ? <FaChevronUp className="text-gray-400" /> : <FaChevronDown className="text-gray-400" />}
                </div>
              </div>
            </div>

            {/* Book Details - Shown when expanded */}
            {expandedRequest === request.id && (
              <div className="border-t border-gray-200 p-4">
                <div className="mb-4">
                  <div className="grid grid-cols-4 gap-4 font-medium mb-2 px-2">
                    <span>Book</span>
                    <span>Status</span>
                    <span>Due Date</span>
                    <span>Return Date</span>
                  </div>
                  <div className="space-y-3">
                    {request.bookBorrowingRequestDetails.map((book, index) => (
                      <div key={index} className="grid grid-cols-4 gap-4 items-center bg-gray-50 p-2 rounded">
                        <span className="flex items-center">
                          <FaBook className="mr-2 text-gray-400" />
                          {book.book.title}
                        </span>
                        <span
                          className={`px-2 py-1 rounded-full text-sm ${
                            request.status === 0 ? "bg-green-100 text-green-700" : "bg-blue-100 text-blue-700"
                          }`}
                        >
                          {request.status === 0 ? "Approved" : "Waiting"}
                        </span>
                        <span className="flex items-center">
                          <FaCalendarCheck className="mr-2 text-gray-400" />
                          {new Date(request.dateExpired).toLocaleDateString()}
                        </span>
                        {/* <span>{book.returnDate}</span> */}
                      </div>
                    ))}
                  </div>
                </div>

                {request.approveName && (
                  <div className="pt-3 border-t border-gray-200 text-sm text-gray-500 flex items-center">
                    <FaCheckCircle className="mr-2 text-green-500" />
                    Processed by: {request.approveName} | {request.updatedAt}
                  </div>
                )}
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Pagination Component */}
      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={handlePageChange}
        itemsPerPage={itemsPerPage}
        onItemsPerPageChange={handleItemsPerPageChange}
        totalItems={requests.length} // Optional: Adjust based on API response
        showPageInfo
      />
    </div>
  );
};

export default BorrowingRequests;
