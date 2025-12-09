export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface LoginResponseDto {
  token: string;
  email: string;
  fullName: string | null;
  expiresAt: Date
}

export interface UnauthorizedResponseDto {
  message: string;
}

export type AuthenticationResponse = LoginResponseDto | UnauthorizedResponseDto;
