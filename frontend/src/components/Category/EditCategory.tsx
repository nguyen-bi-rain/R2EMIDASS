import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { categoryRequestSchema } from '../../utils/validations/categoryValidation';
import Input from '../ui/Input';
import TextArea from '../ui/TextArea';
import Button from '../ui/Button';
import { CategoryType } from '../../models/categoryType';
import categoryService from '../../services/category.service';
import { toast } from 'react-toastify';
import { useNavigate, useParams } from 'react-router-dom';

const EditCategory = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [defaultValues, setDefaultValues] = useState<Partial<CategoryType>>({});

    const {
        register,
        handleSubmit,
        formState: { errors },
        reset, // Used to reset the form with default values
    } = useForm<CategoryType>({
        resolver: zodResolver(categoryRequestSchema),
        defaultValues: defaultValues as Partial<CategoryType>, // Set default values here
    });

    const onSubmit = async (data: CategoryType) => {
        try {
            console.log(data);
            
            await categoryService.updateCategory(id, data);
            toast.success("Category updated successfully!");
            navigate(-1);
        } catch (error) {
            toast.error("Failed to update category. Please try again.");
            console.error("Error updating category:", error);
        }
    };

    useEffect(() => {
        const fetchCategory = async () => {
            if (id) {
                try {
                    const res = await categoryService.getCategoryById(id);
                    console.log(res.data);
                    setDefaultValues(res.data); // Set the fetched data as default values
                    reset(res.data); // Reset the form with the fetched data
                } catch (error) {
                    console.error("Error fetching category:", error);
                }
            }
        };
        fetchCategory();
    }, [id, reset]);

    const handleCancel = (e?: React.MouseEvent<HTMLButtonElement>) => {
        e?.preventDefault();
        navigate(-1);
    }

    return (
        <div className='bg-white p-4 rounded shadow-md max-w-3xl mx-auto mt-8 mb-18'>
            <h1 className="text-2xl font-bold mb-4 text-center">Edit Category</h1>
            <form onSubmit={handleSubmit(onSubmit)}>
                <Input
                    id="name"
                    label="Category Name"
                    type="text"
                    placeholder="Enter category name"
                    register={register}
                    error={errors.name}
                    className="mb-4"
                />
                <TextArea
                    row={5}
                    id="description"
                    label="Description"
                    placeholder="Enter category description"
                    register={register}
                    error={errors.description}
                    className="mb-4"
                />
                <div className="flex justify-end gap-4">
                    <Button type="button" variant="secondary" size="md" className="w-full max-w-3/12" onClick={handleCancel}>
                        Cancel
                    </Button>

                    <Button type="submit" variant="primary" size="md" className="w-full max-w-3/12">
                        Save
                    </Button>
                </div>
            </form>
        </div>
    );
};

export default EditCategory;