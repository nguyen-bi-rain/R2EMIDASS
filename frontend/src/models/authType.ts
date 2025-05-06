export type LoginRequest = {
    userName: string;
    password: string;
}

export type LoginResponse = {
    accessToken: string;
    userId : string;
    roleName: string;
}

export type RegisterRequest = {
    email: string;
    password: string;
    confirmPassword: string;
    userName : string;
    gender: number;
    phoneNumber: string
}
