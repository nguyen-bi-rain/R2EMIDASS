import React from "react";
import { HiChevronDoubleLeft, HiChevronLeft, HiChevronRight, HiChevronDoubleRight } from "react-icons/hi";

type PaginationProps = {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    className?: string;
    showPageNumbers?: boolean;
    showPageInfo?: boolean;
    totalItems?: number;
    itemsPerPage?: number;
    onItemsPerPageChange: (itemsPerPage: number) => void;
};

const Pagination: React.FC<PaginationProps> = ({
    currentPage,
    totalPages,
    onPageChange,
    className = "",
    showPageNumbers = true,
    showPageInfo = false,
    totalItems,
    itemsPerPage,
    onItemsPerPageChange,
}) => {
    const maxVisiblePages = 5;

    const getPageNumbers = () => {
        if (totalPages <= maxVisiblePages) {
            return Array.from({ length: totalPages }, (_, i) => i + 1);
        }

        const half = Math.floor(maxVisiblePages / 2);
        let start = currentPage - half;
        let end = currentPage + half;

        if (start < 1) {
            start = 1;
            end = maxVisiblePages;
        }

        if (end > totalPages) {
            end = totalPages;
            start = totalPages - maxVisiblePages + 1;
        }

        const pages = [];
        for (let i = start; i <= end; i++) {
            pages.push(i);
        }

        return pages;
    };

    const pageNumbers = getPageNumbers();

    return (
        <div className={`flex flex-col sm:flex-row items-center justify-between gap-4 ${className}`}>
            <div className="flex items-center space-x-2">
                {showPageInfo && totalItems && itemsPerPage && (
                    <div className="text-sm text-gray-600 whitespace-nowrap">
                        Showing <span className="font-medium">{(currentPage - 1) * itemsPerPage + 1}</span> to{" "}
                        <span className="font-medium">{Math.min(currentPage * itemsPerPage, totalItems)}</span> of{" "}
                        <span className="font-medium">{totalItems}</span> items
                    </div>
                )}
                <div className="flex items-center space-x-2">
                    <label htmlFor="itemsPerPage" className="text-sm text-gray-600 whitespace-nowrap">
                        Items per page:
                    </label>
                    <select
                        id="itemsPerPage"
                        value={itemsPerPage}
                        onChange={(e) => {
                            const newItemsPerPage = Number(e.target.value);
                            onItemsPerPageChange(newItemsPerPage);
                            onPageChange(1);
                        }}
                        className="text-sm border border-gray-300 rounded-md px-2 py-1 focus:ring-blue-500 focus:border-blue-500"
                    >
                        {[4, 6, 12, 24].map((size) => (
                            <option key={size} value={size}>
                                {size}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            <div className="flex items-center gap-4">
                <div className="flex items-center space-x-1">
                    <button
                        onClick={() => onPageChange(1)}
                        disabled={currentPage === 1}
                        className={`p-2 rounded-md ${currentPage === 1
                            ? "text-gray-300 cursor-not-allowed"
                            : "text-gray-600 hover:bg-gray-100 hover:text-gray-900 transition-colors"
                            }`}
                        aria-label="First page"
                    >
                        <HiChevronDoubleLeft className="h-5 w-5" />
                    </button>

                    <button
                        onClick={() => onPageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                        className={`p-2 rounded-md ${currentPage === 1
                            ? "text-gray-300 cursor-not-allowed"
                            : "text-gray-600 hover:bg-gray-100 hover:text-gray-900 transition-colors"
                            }`}
                        aria-label="Previous page"
                    >
                        <HiChevronLeft className="h-5 w-5" />
                    </button>

                    {showPageNumbers && (
                        <div className="flex items-center space-x-1">
                            {pageNumbers[0] > 1 && (
                                <>
                                    <button
                                        onClick={() => onPageChange(1)}
                                        className={`px-3 py-1 rounded-md ${currentPage === 1
                                            ? "bg-blue-600 text-white"
                                            : "text-gray-700 hover:bg-gray-100 transition-colors"
                                            }`}
                                    >
                                        1
                                    </button>
                                    {pageNumbers[0] > 2 && (
                                        <span className="px-2 text-gray-400">...</span>
                                    )}
                                </>
                            )}

                            {pageNumbers.map((page) => (
                                <button
                                    key={page}
                                    onClick={() => onPageChange(page)}
                                    className={`px-3 py-1 rounded-md min-w-[2.5rem] ${currentPage === page
                                        ? "bg-blue-600 text-white"
                                        : "text-gray-700 hover:bg-gray-100 transition-colors"
                                        }`}
                                >
                                    {page}
                                </button>
                            ))}

                            {pageNumbers[pageNumbers.length - 1] < totalPages && (
                                <>
                                    {pageNumbers[pageNumbers.length - 1] < totalPages - 1 && (
                                        <span className="px-2 text-gray-400">...</span>
                                    )}
                                    <button
                                        onClick={() => onPageChange(totalPages)}
                                        className={`px-3 py-1 rounded-md ${currentPage === totalPages
                                            ? "bg-blue-600 text-white"
                                            : "text-gray-700 hover:bg-gray-100 transition-colors"
                                            }`}
                                    >
                                        {totalPages}
                                    </button>
                                </>
                            )}
                        </div>
                    )}

                    <button
                        onClick={() => onPageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                        className={`p-2 rounded-md ${currentPage === totalPages
                            ? "text-gray-300 cursor-not-allowed"
                            : "text-gray-600 hover:bg-gray-100 hover:text-gray-900 transition-colors"
                            }`}
                        aria-label="Next page"
                    >
                        <HiChevronRight className="h-5 w-5" />
                    </button>

                    <button
                        onClick={() => onPageChange(totalPages)}
                        disabled={currentPage === totalPages}
                        className={`p-2 rounded-md ${currentPage === totalPages
                            ? "text-gray-300 cursor-not-allowed"
                            : "text-gray-600 hover:bg-gray-100 hover:text-gray-900 transition-colors"
                            }`}
                        aria-label="Last page"
                    >
                        <HiChevronDoubleRight className="h-5 w-5" />
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Pagination;
