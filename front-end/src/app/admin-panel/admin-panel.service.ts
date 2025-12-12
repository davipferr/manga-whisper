import { Injectable } from '@angular/core';
import { ManualCheckResponseDto } from './admin-panel.model';
import { StatusMessage } from "./admin-panel.model";

@Injectable({
  providedIn: 'root'
})
export class AdminPanelService {

  handleSetStatusMessage(response: ManualCheckResponseDto): StatusMessage {
    this.checkResponseMessage(response);
    return this.setStatusMessage(response);
  }

  private checkResponseMessage(response: ManualCheckResponseDto): void {
    if (!response.success) {
      response.message = response.errorMessage ?? 'An error occurred during the chapter check.';
      return;
    }

    response.message = response.message ?? 'Chapter check completed successfully!';
  }

  private setStatusMessage(response: ManualCheckResponseDto): StatusMessage {
    let statusMessage: StatusMessage = { message: '' };

    if (!response.success) {
      this.setErrorMessage(statusMessage, response);
      return statusMessage;
    }

    this.setSuccessMessage(statusMessage, response);
    return statusMessage;
  }

  private setSuccessMessage(statusMessage: StatusMessage, response: Readonly<ManualCheckResponseDto>): void {
    statusMessage.message = response.message ?? 'Chapter check completed successfully!';

    if (response.newChapters && response.newChapters.length > 0) {
      statusMessage.message += '. New chapters found: ' + response.newChapters.map(chapter => chapter.number).join(', ');
    }
  }

  private setErrorMessage(statusMessage: StatusMessage, response: Readonly<ManualCheckResponseDto>): void {
    statusMessage.message = response.errorMessage ?? 'An error occurred during the chapter check.';
    statusMessage.isError = true;
  }

  setWarningMessage(statusMessage: string): StatusMessage {
    let status: StatusMessage = { message: statusMessage, isWarning: true };
    return status;
  }
}
