import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { error } from 'util';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() valueFormHome: any;
  @Output() cancelRegister = new EventEmitter();

  model: any = {};
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }
  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log("registeration successful");
    }, error => { console.log(error); });
  }
  cancel() {
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }

}
