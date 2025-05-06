import { useEffect, useState } from 'react';
import Button from '../../components/ui/Button';
import categoryService from '../../services/category.service';
import Spinner from '../../components/ui/Spinner';
import DataTable from '../../components/ui/TableData';
import Pagination from '../../components/ui/Pagination';
import { useNavigate } from 'react-router-dom';
import Dialog from '../../components/ui/Dialog';
import { toast } from 'react-toastify';

const Category = () => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(12);
  const [totalItems, setTotalItems] = useState(0);
  const [showDialog, setShowDialog] = useState(false);
  const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchCategories = async () => {
      setLoading(true);
      try {
        const res = await categoryService.getAllCategories(page, itemsPerPage);
        setCategories(res.data.items);
        setTotalItems(res.data.totalCount);
        console.log(res.data);
      } catch (error) {
        console.error("Error fetching categories:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchCategories();
  }, [page, itemsPerPage]);

  const handleDeleteCategory = async (id: string) => {
    try {
      await categoryService.deleteCategory(id);
      window.location.reload();
      toast.success("Category deleted successfully!");
    } catch (error) {
      console.error("Error deleting category:", error);
      toast.error("Failed to delete category.");
    }
  };

  const handleOpenDialog = (id: string) => {
    console.log("Open dialog for category ID:");
    setSelectedCategoryId(id);
    setShowDialog(true);
    console.log(showDialog);
    
  };

  const handleDialogClose = (confirmed: boolean) => {
    console.log("Dialog closed:", confirmed);
    setShowDialog(false);
    if (confirmed && selectedCategoryId) {
      handleDeleteCategory(selectedCategoryId);
    }
  };

  const columns = [
    { key: "name", title: "Name", sortable: true, width: "200px", align: "left" },
    { key: "description", title: "Description", sortable: true, width: "500px", align: "left" },
    {
      key: "action",
      title: "Action",
      sortable: false,
      width: "100px",
      align: "center",
      render: (row: any) => (
        <div className="flex justify-center">
            <Button variant="secondary" size="sm" onClick={() => navigate(`/admin/edit-category/${row.id}`)} >
            Edit
            </Button>
          <Button
            className="bg-red-500 text-white px-2 py-1 rounded ml-2"
            onClick={() => handleOpenDialog(row.id)}
          >
            Delete
          </Button>
        </div>
      ),
    },
  ];

  if (loading) {
    return <Spinner />;
  }

  return (
    <div>
      <div className="flex justify-end items-center mb-4">
        <Button variant="primary" size="md" onClick={() => navigate("/admin/create-category")}>
          Add Category
        </Button>
      </div>

      <DataTable
        striped={true}
        bordered={true}
        className="mb-4"
        columns={columns}
        data={categories}
        sortable={true}
        page={page}
        itemsPerPage={itemsPerPage}
        totalItems={totalItems}
        onPageChange={(newPage) => setPage(newPage)}
        onItemsPerPageChange={(newItemsPerPage) => setItemsPerPage(newItemsPerPage)}
      />
      <Pagination
        showPageInfo={true}
        totalPages={Math.ceil(totalItems / itemsPerPage)}
        currentPage={page}
        totalItems={totalItems}
        itemsPerPage={itemsPerPage}
        onPageChange={(newPage) => setPage(newPage)}
        onItemsPerPageChange={(newItemsPerPage) => setItemsPerPage(newItemsPerPage)}
      />

      {showDialog == true ? <Dialog onClose={handleDialogClose} /> : null}
    </div>
  );
};

export default Category;