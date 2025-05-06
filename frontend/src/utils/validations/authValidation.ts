import { z } from "zod";
import { LoginRequest, RegisterRequest } from "../../models/authType";

export const loginRequestSchema = z.object({
    userName: z.string().min(3, "Username must be at least 3 characters long"),
    password: z.string().min(6, "Password must be at least 6 characters long"),
});

// Example usage
export const validateLoginRequest = (data: LoginRequest) => {
    return loginRequestSchema.safeParse(data);
};

export const registerRequestSchema = z.object({
    phoneNumber: z.string().min(10, "Phone number must be at least 10 characters long"),
    email: z.string().email("Invalid email format"),
    password: z.string().min(6, "Password must be at least 6 characters long"),
    confirmPassword: z.string().min(6, "confirmPassword must be at least 6 characters long"),
    userName: z.string().min(3, "Username must be at least 3 characters long"),
    gender : z.string().nonempty()
}).refine(data => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
})

export const validateRegisterRequest = (data: RegisterRequest) => {
    return registerRequestSchema.safeParse(data);
}