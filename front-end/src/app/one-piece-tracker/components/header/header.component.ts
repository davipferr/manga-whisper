import { Component } from '@angular/core';

@Component({
  selector: 'app-header',
  template: `
    <header class="header">
      <div class="header-left">
        <div class="logo">
          <div class="logo-icon">⚪</div>
          <div class="logo-text">
            <h1>One Piece Tracker</h1>
            <p>Never miss a chapter release</p>
          </div>
        </div>
      </div>
      <div class="header-right">
        <button class="settings-btn">
          Settings
        </button>
      </div>
    </header>
  `,
  styles: [`
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(20px);
      border-radius: 16px;
      padding: 20px 30px;
      margin-bottom: 30px;
      border: 1px solid rgba(255, 255, 255, 0.2);
    }

    .logo {
      display: flex;
      align-items: center;
      gap: 15px;
    }

    .logo-icon {
      width: 40px;
      height: 40px;
      background: #ff4444;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 20px;
    }

    .logo-text h1 {
      color: white;
      font-size: 24px;
      font-weight: 600;
      margin-bottom: 2px;
    }

    .logo-text p {
      color: rgba(255, 255, 255, 0.8);
      font-size: 14px;
      margin-bottom: 0;
    }

    .settings-btn {
      background: rgba(255, 255, 255, 0.1);
      border: 1px solid rgba(255, 255, 255, 0.2);
      color: white;
      padding: 12px 20px;
      border-radius: 8px;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s ease;
    }

    .settings-btn:hover {
      background: rgba(255, 255, 255, 0.2);
    }

    @media (max-width: 768px) {
      .header {
        flex-direction: column;
        gap: 20px;
        text-align: center;
      }
    }
  `]
})
export class HeaderComponent {
}
