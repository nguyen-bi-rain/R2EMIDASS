// filepath: f:/NashTech/MidAssignment/frontend/src/components/Book/CreateBook.tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { bookRequestSchema } from '../../utils/validations/bookValidation';
import Input from '../ui/Input';
import TextArea from '../ui/TextArea';
import Button from '../ui/Button';
import { BookType } from '../../models/bookType';
import bookService from '../../services/book.service';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { CustomSelect } from '../ui/Select';
import categoryService from '../../services/category.service';
import { useEffect, useState } from 'react';

const CreateBook = () => {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState<string | number | undefined>(0);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<BookType>({
    resolver: zodResolver(bookRequestSchema),
  });

  const onSubmit = async (data: BookType) => {
    try {
      console.log(data);

      await bookService.createBook({ ...data, categoryId: selectedCategory as number, available: data.quantity });
      toast.success("Book created successfully!");
      navigate(-1);
    } catch (error) {
      toast.error("Failed to create book. Please try again.");
      console.error("Error creating book:", error);
    }
    
  };

  useEffect(() => {
    fetchCategories();
  }, [])

  const fetchCategories = async () => {
    const res = await categoryService.getAllCategories(1, -1);
    const options = res.data.items.map((category: any) => ({
      value: category.id,
      label: category.name,
    }));
    setCategories(options);
  };

  const handleCategoryChange = (value: string | number) => setSelectedCategory(value);

  return (
    <div className="bg-white p-4 rounded shadow-md max-w-3xl mx-auto mt-8 mb-18">
      <h1 className="text-2xl font-bold mb-4 text-center">Create Book</h1>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Input
          id="title"
          label="Title"
          type="text"
          placeholder="Enter book title"
          register={register}
          error={errors.title}
          className="mb-4"
        />
        <TextArea
          id="description"
          label="Description"
          placeholder="Enter book description"
          register={register}
          error={errors.description}
          row={5}
          className="mb-4"
        />
        <Input
          id="author"
          label="Author"
          type="text"
          placeholder="Enter author name"
          register={register}
          error={errors.author}
          className="mb-4"
        />
        <Input
          defaultValue={0}
          id="quantity"
          label="Quantity"
          type="number" 
          placeholder="Enter quantity"
          register={register}
          error={errors.quantity}
          
          className="mb-4"
        />
        <Input
          id="publishedDate"
          label="Published Date"
          type="date"
          placeholder="Select published date"
          register={register}
          error={errors.publishedDate}
          className="mb-4"
        />
        <CustomSelect
          options={categories}
          placeholder="Select Category"
          className="w-full mb-4"
          onChange={handleCategoryChange}
          value={selectedCategory}
        />
        <div className="flex justify-end gap-4">
          <Button type="button" variant="secondary" size="md" className="w-full max-w-3/12" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button type="submit" variant="primary" size="md" className="w-full max-w-3/12">
            Create
          </Button>
        </div>
      </form>
    </div>
  );
};

export default CreateBook;
