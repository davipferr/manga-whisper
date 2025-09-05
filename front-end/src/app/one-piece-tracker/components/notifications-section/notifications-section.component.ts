import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-notifications-section',
  imports: [FormsModule],
  template: `
    <div class="notifications-section">
      <div class="notification-item">
        <div class="notification-icon">📱</div>
        <div class="notification-content">
          <h3>WhatsApp Integration</h3>
          <p>Receive notifications when new chapters are published directly to your WhatsApp</p>
        </div>
        <div class="toggle-switch">
          <input type="checkbox" id="alerts-toggle" [(ngModel)]="alertsEnabled" />
          <label for="alerts-toggle" class="switch"></label>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .notifications-section {
        background: rgba(255, 255, 255, 0.95);
        border-radius: 16px;
        padding: 30px;
        margin-bottom: 40px;
        box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
      }

      .notification-item {
        display: flex;
        align-items: center;
        gap: 20px;
        padding: 20px 0;
        border-bottom: 1px solid #eee;
      }

      .notification-item:last-child {
        border-bottom: none;
      }

      .notification-icon {
        font-size: 24px;
        width: 50px;
        height: 50px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #f8f9ff;
        border-radius: 12px;
      }

      .notification-content {
        flex: 1;
      }

      .notification-content h3 {
        color: #333;
        font-size: 16px;
        font-weight: 600;
        margin-bottom: 4px;
      }

      .notification-content p {
        color: #666;
        font-size: 14px;
      }

      .toggle-switch {
        position: relative;
      }

      .toggle-switch input[type='checkbox'] {
        display: none;
      }

      .switch {
        position: relative;
        display: inline-block;
        width: 50px;
        height: 24px;
        background: #ccc;
        border-radius: 12px;
        cursor: pointer;
        transition: background 0.3s;
      }

      .switch::after {
        content: '';
        position: absolute;
        top: 2px;
        left: 2px;
        width: 20px;
        height: 20px;
        background: white;
        border-radius: 50%;
        transition: transform 0.3s;
      }

      input[type='checkbox']:checked + .switch {
        background: #ff4444;
      }

      input[type='checkbox']:checked + .switch::after {
        transform: translateX(26px);
      }
    `,
  ],
})
export class NotificationsSectionComponent {
  alertsEnabled = signal(true);
}
