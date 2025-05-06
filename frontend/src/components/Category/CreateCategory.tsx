import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { categoryRequestSchema } from '../../utils/validations/categoryValidation';
import Input from '../ui/Input';
import TextArea from '../ui/TextArea';
import Button from '../ui/Button';
import { CategoryType } from '../../models/categoryType';
import categoryService from '../../services/category.service';
import { toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';

const CreateCategory = () => {

    const navigate = useNavigate();
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<CategoryType>({
        resolver: zodResolver(categoryRequestSchema),
    });

    const onSubmit = async (data: CategoryType) => {
        try {
            console.log(data);
            
            await categoryService.createCategory(data);
            toast.success("Category created successfully!");
            navigate(-1)
        } catch (error) {
            toast.error("Failed to create category. Please try again.");
            console.error("Error creating category:", error);
        }
    }
    return (
        <div className='bg-white p-4 rounded shadow-md max-w-3xl mx-auto mt-8 mb-18'>
            <h1 className="text-2xl font-bold mb-4 text-center">Create Category</h1>
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
                    register={register}
                    row={5}
                    error={errors.description}
                    id="description"
                    label="Description"
                    placeholder="Enter category description"
                    className="mb-4"
                />
                <div className="flex justify-end gap-4">
                    <Button type="submit" variant="secondary" size="md" className="w-full max-w-3/12">
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

export default CreateCategory;