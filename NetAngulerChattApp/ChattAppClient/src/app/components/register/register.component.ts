import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterModel } from '../../Models/register.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerModel: RegisterModel = new RegisterModel();

  constructor(private http: HttpClient,
    private router : Router
  ){

  }

  setImage(event:any){
    console.log(event);
    this.registerModel.file = event.target.files[0];
  }

  register(){
    const formdata = new FormData();
    formdata.append("name",this.registerModel.name);
    formdata.append("file",this.registerModel.file, this.registerModel.file.new);

    this.http.post("https://localhost:7003/api/Auth/Register", formdata).subscribe(res=>{
      localStorage.setItem("accessToken",JSON.stringify(res));
      this.router.navigateByUrl("/");
    })
  }

}
