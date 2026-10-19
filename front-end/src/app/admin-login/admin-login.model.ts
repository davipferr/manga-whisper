export interface LoginRequestDto {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface LoginResponseDto {
  token: string;
  email: string;
  fullName: string | null;
  expiresAt: Date;
  roles: string[];
}

export interface UnauthorizedResponseDto {
  message: string;
}

export type AuthenticationResponse = LoginResponseDto | UnauthorizedResponseDto;
