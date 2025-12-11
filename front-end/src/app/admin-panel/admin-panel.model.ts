export interface RoleCheckRequestDto {
  email: string;
  roleName: string;
}

export interface RoleCheckResponseDto {
  hasRole: boolean;
  message: string;
}

export interface ManualCheckResponseDto {
  success: boolean;
  message?: string;
  errorMessage?: string;
}
