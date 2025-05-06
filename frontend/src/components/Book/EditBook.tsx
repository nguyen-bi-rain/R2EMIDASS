import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { bookRequestSchema } from '../../utils/validations/bookValidation';
import Input from '../ui/Input';
import TextArea from '../ui/TextArea';
import Button from '../ui/Button';
import { BookType } from '../../models/bookType';
import bookService from '../../services/book.service';
import categoryService from '../../services/category.service';
import { toast } from 'react-toastify';
import { useNavigate, useParams } from 'react-router-dom';
import { CustomSelect } from '../ui/Select';
import Spinner from '../ui/Spinner';

const EditBook = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState<string | number | null>(null);
    const [defaultValues, setDefaultValues] = useState<Partial<BookType>>({});
    const [isLoading, setIsLoading] = useState(false);
    const [isSaving, setIsSaving] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
        reset,
    } = useForm<BookType>({
        resolver: zodResolver(bookRequestSchema),
        defaultValues: defaultValues
    });

    const onSubmit = async (data: BookType) => {
        console.log('Form data:', data);    
        setIsSaving(true);
        try {
            await bookService.updateBook(id, { ...data, categoryId: selectedCategory as number, available: defaultValues.available });
            toast.success('Book updated successfully!');
            navigate(-1);
        } catch (error) {
            toast.error('Failed to update book. Please try again.');
            console.error('Error updating book:', error);
        } finally {
            setIsSaving(false);
        }
    };

    useEffect(() => {
        const fetchBook = async () => {
            setIsLoading(true);
            try {
                const res = await bookService.getBookById(id);
                const bookData = res.data;
                setDefaultValues(bookData);
                setSelectedCategory(bookData.categoryId);
                reset({
                    ...bookData,
                    publishedDate: bookData.publishedDate
                        ? new Date(bookData.publishedDate).toISOString().split('T')[0]
                        : '',
                });
            } catch (error) {
                console.error('Error fetching book:', error);
            } finally {
                setIsLoading(false);
            }
        };

        const fetchCategories = async () => {

            try {
                const res = await categoryService.getAllCategories(1, -1);
                const options = res.data.items.map((category: any) => ({
                    value: category.id,
                    label: category.name,
                }));
                setCategories(options);
            } catch (error) {
                console.error('Error fetching categories:', error);
            }
        };

        fetchCategories();
        fetchBook();
    }, [id, reset]);

    const handleCategoryChange = (value: string | number) => setSelectedCategory(value);

    const handleCancel = (e?: React.MouseEvent<HTMLButtonElement>) => {
        e?.preventDefault();
        navigate(-1);
    };

    if (isLoading) {
        return (
            <div className="flex justify-center items-center h-screen">
                <Spinner />
            </div>
        );
    }

    return (
        <div className="bg-white p-4 rounded shadow-md max-w-3xl mx-auto mt-8 mb-18">
            <h1 className="text-2xl font-bold mb-4 text-center">Edit Book</h1>
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
                    defaultValue={defaultValues.quantity}
                    id="quantity"
                    label="Quantity"
                    type="number"
                    placeholder="Enter quantity"
                    register={register}
                    error={errors.quantity}
                    className="mb-4"
                />
                <div className="mb-4">
                    <label htmlFor="available" className="block text-sm font-medium text-gray-700 mb-1">
                        Available
                    </label>
                    <input
                        defaultValue={defaultValues.available}
                        placeholder="Enter available quantity"
                        type="number"
                        id="available"
                        {...register('available')}
                        disabled
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    />
                </div>
                <div className="mb-4">
                    <label htmlFor="publishedDate" className="block text-sm font-medium text-gray-700 mb-1">
                        Published Date
                    </label>
                    <input
                        defaultValue={defaultValues.publishedDate}
                        placeholder="Enter published date"
                        type="date"
                        id="publishedDate"
                        {...register('publishedDate')}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
                    />
                </div>
                <CustomSelect
                    options={categories}
                    placeholder="Select Category"
                    className="w-full mb-4"
                    onChange={handleCategoryChange}
                    value={selectedCategory || undefined}
                />
                <div className="flex justify-end gap-4">
                    <Button type="button" variant="secondary" size="md" className="w-full max-w-3/12" onClick={handleCancel} disabled={isSaving}>
                        Cancel
                    </Button>
                    <Button type="submit" variant="primary" size="md" className="w-full max-w-3/12" disabled={isSaving}>
                        {isSaving ? <Spinner /> : 'Save'}
                    </Button>
                </div>
            </form>
        </div>
    );
};

export default EditBook;