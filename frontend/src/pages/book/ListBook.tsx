import { useEffect, useState } from "react";
import Button from "../../components/ui/Button";
import Pagination from "../../components/ui/Pagination";
import DataTable from "../../components/ui/TableData";
import bookService from "../../services/book.service";
import { CustomSelect } from "../../components/ui/Select";
import categoryService from "../../services/category.service";
import { formatCustomDate } from "../../helpers/formatDateHelp";
import Spinner from "../../components/ui/Spinner";
import useDebounce from "../../hooks/useDebounce"; // Import the useDebounce hook
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom";
import Dialog from "../../components/ui/Dialog";

const ListBook = () => {
    const navigate = useNavigate();
    const [books, setBooks] = useState([]);
    const [loading, setLoading] = useState(false);
    const [page, setPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(12);
    const [totalItems, setTotalItems] = useState(0);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState<string | number | undefined>(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [showDialog, setShowDialog] = useState(false);
    const [bookId, setBookId] = useState<string | null>(null);
    const debouncedSearchTerm = useDebounce(searchTerm, 1000);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                await Promise.all([fetchBooks(), fetchCategories()]);
            } catch (error) {
                console.error("Error fetching data:", error);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [page, itemsPerPage, selectedCategory, debouncedSearchTerm]);

    const fetchBooks = async () => {
        const res = await bookService.getAllBooks(page, itemsPerPage, debouncedSearchTerm, selectedCategory as string);
        setBooks(res.data.items);
        setTotalItems(res.data.totalCount);
    };

    const fetchCategories = async () => {
    try {
        const res = await categoryService.getAllCategories(page, -1);
        const options = [
            { value: 0, label: "All" },
            ...res.data.items.map((category: any) => ({
                value: category.id,
                label: category.name,
            }))
        ];
        setCategories(options);
    } catch (error) {
        toast.error("Failed to fetch categories. Please try again.");
        console.error("Error fetching categories:", error);
    }
    };

    const handleCategoryChange = (value: string | number) => setSelectedCategory(value);

    const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => setSearchTerm(event.target.value);
    const handleDeleteCategory = async (id: string) => {
        try {
            await bookService.deleteBook(id);
            toast.success("Book deleted successfully!");

        } catch (error: any) {
            console.error("Error deleting book:", error);
            toast.error(error.response?.data?.message || "Failed to delete book.");
        }
    }
    const handleDialogClose = (confirmed: boolean) => {
        console.log("Dialog closed:", confirmed);
        setShowDialog(false);
        if (confirmed && bookId) {
            handleDeleteCategory(bookId);
        }
    };
    const handleOpenDialog = (id: string) => {
        console.log("Open dialog for book ID:");
        setBookId(id);
        setShowDialog(true);
    };
    const columns = [
        { key: "title", title: "Title", sortable: true, width: "200px", align: "left" },
        { key: "author", title: "Author", sortable: true, width: "100px", align: "left" },
        {
            key: "publishedDate",
            title: "Published Date",
            sortable: true,
            width: "150px",
            align: "center",
            render: (item) => (
                <div className="text-center">
                    <span className="text-gray-500">{formatCustomDate(new Date(item.publishedDate))}</span>
                </div>
            ),
        },
        {
            key: "available",
            title: "Available",
            sortable: true,
            width: "100px",
            align: "center",
            render: (item) => (
                <div className="flex justify-center items-center">
                    <span
                        className={`px-2 py-1 text-sm font-medium rounded-full ${item.available ? "text-green-700 bg-green-100" : "text-red-700 bg-red-100"
                            }`}
                    >
                        {item.available ? item.available : "Not Available"}
                    </span>
                </div>
            ),
        },
        {
            key: "action",
            title: "Action",
            sortable: false,
            width: "100px",
            align: "center",
            render: (row: any) => (
                <div className="flex justify-center">
                    <Button variant="secondary" size="sm" onClick={() => navigate(`/admin/edit-book/${row.id}`)}>
                        Edit
                    </Button>
                    <Button variant="danger" size="sm" className="ml-2" onClick={() => handleOpenDialog(row.id)}>Delete</Button>
                </div>
            ),
        },
    ];

    if (loading) {
        return (
            <Spinner />
        );
    }

    return (
        <div>
            <div className="flex justify-end items-center flex-wrap mt-4 gap-4">
                <div className="flex items-center gap-2">
                    <input
                        type="text"
                        placeholder="Search books by title"
                        className="border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        onChange={handleSearchChange}
                        value={searchTerm}
                    />
                    <CustomSelect
                        options={categories}
                        placeholder="Filter by Category"
                        className="w-40"
                        onChange={handleCategoryChange}
                        value={selectedCategory}
                    />
                    <Button 
                        variant="secondary" 
                        size="sm" 
                        onClick={() => {
                            setSearchTerm("");
                            setSelectedCategory(0);
                        }}
                    >
                        Reset
                    </Button>
                </div>
                <Button variant="primary" size="md" onClick={() => navigate("/admin/create-book")}>
                    Add Book
                </Button>
            </div>
            <DataTable
                columns={columns}
                data={books}
                sortable
                striped
                hoverable
                className="mt-4"
                rowKey={(item, index) => index}
                emptyState={<div className="py-8 text-center text-gray-500">No data available</div>}
            />
            {books.length > 0 && (
                <Pagination
                    className="mt-4"
                    currentPage={page}
                    totalPages={Math.ceil(totalItems / itemsPerPage)}
                    onPageChange={setPage}
                    itemsPerPage={itemsPerPage}
                    onItemsPerPageChange={(newItemsPerPage) => {
                        setItemsPerPage(newItemsPerPage);
                        setPage(1); // Reset to the first page when items per page changes
                    }}
                    totalItems={totalItems}
                    showPageInfo
                />
            )}
            {showDialog == true ? <Dialog onClose={handleDialogClose} /> : null}

        </div>
    );
};

export default ListBook;
