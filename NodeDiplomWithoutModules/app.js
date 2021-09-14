const mysql = require("mysql2");
const express = require("express");
const bodyParser = require("body-parser");
const hbs = require('hbs');
const app = express();
const session  = require('express-session');
const urlencodedParser = bodyParser.urlencoded({extended: true});
const fs = require("fs");
const multer  = require("multer");
//app.use(multer({dest:"views/imagines"}).single("file"));
const storageConfig = multer.diskStorage({
    destination: (req, file, cb) =>{
        cb(null, "views/imagines");
    },
    filename: (req, file, cb) =>{
        cb(null, file.originalname);
    }
});
app.use(multer({storage:storageConfig}).single("file"));
app.use(bodyParser.json({ limit: "50mb" }));
app.use(bodyParser.urlencoded({ limit: "50mb", extended: true, parameterLimit: 50000 }))
app.use(express.static('views'));

app.use(session({
  secret: 'keyboard cat',
  resave: false,
  saveUninitialized: true,
}))



const pool = mysql.createPool({
  connectionLimit: 5,
  host: "127.0.0.1",
  user: "root",
  database: "mybd",
  password: ""
});
 
app.set("view engine", "hbs");
hbs.registerHelper('if_eq', function(a, b, opts) {
if (a == b) {
	return opts.fn(this);
} else {
	return opts.inverse(this);
}
});

hbs.registerHelper('compare', function (lvalue, operator, rvalue, options) {

    var operators, result;

    if (arguments.length < 3) {
        throw new Error("Handlerbars Helper 'compare' needs 2 parameters");
    }

    if (options === undefined) {
        options = rvalue;
        rvalue = operator;
        operator = "===";
    }

    operators = {
        '==': function (l, r) { return l == r; },
        '===': function (l, r) { return l === r; },
        '!=': function (l, r) { return l != r; },
        '!==': function (l, r) { return l !== r; },
        '<': function (l, r) { return l < r; },
        '>': function (l, r) { return l > r; },
        '<=': function (l, r) { return l <= r; },
        '>=': function (l, r) { return l >= r; },
        'typeof': function (l, r) { return typeof l == r; }
    };

    if (!operators[operator]) {
        throw new Error("Handlerbars Helper 'compare' doesn't know the operator " + operator);
    }

    result = operators[operator](lvalue, rvalue);

    if (result) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }

});



hbs.registerPartials(__dirname + "/views/partials");


app.get("/", function(req, res){
	if(typeof(req.session.UserId) != "undefined" && req.session.UserId !== null)
	{
		if(req.session.UserId==45)
		{
			var Logged = "admin";
		}
		else
		{
			var Logged = "true";
		}
	}
	else
	{
		var Logged = "false";
	}
	pool.query("select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie,img from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_tp_tp, Znachenie from tp_tp_type) as second on first.type = second.id_tp_tp", 
	function(err, data) {
		//console.log(req.session.email);
		//console.log(req.session);
		if(err) return console.log(err);
		res.render("index.hbs", {
			tovars: data,
			isLogged: Logged
			
      });
    });
});

app.get("/tovars", function(req, res){
	console.log(req.session);
	if(typeof(req.session.UserId) != "undefined" && req.session.UserId !== null)
	{
		if(req.session.UserId==45)
		{
			var Logged = "admin";
		}
		else
		{
			var Logged = "true";
		}
	}
	else
	{
		var Logged = "false";
	}
    pool.query("select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie,img from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_tp_tp, Znachenie from tp_tp_type) as second on first.type = second.id_tp_tp", 
	function(err, data) {
		if(err) return console.log(err);
		//console.log(data);
		res.render("tovars.hbs", {
			tovars: data,
			isLogged: Logged
		});
    });
});
app.get("/korzina", function(req, res){
	if(typeof(req.session.UserId) != "undefined" && req.session.UserId !== null)
	{
		if(req.session.UserId==45)
		{
			var Logged = "admin";
			var zapros = "select @i:=@i+1 as num, id_fio,First_name,Last_name,id_tovar,id_korzina,Nazvanie,Opisanie,Znachenie,price,kol_vo,(price*kol_vo) as summ, NamePostavshik,DataPokupki,img from (select @i:=0, fio.id_fio,First_name,Last_name,id_korzina,id_tovar as idtovar,kol_vo,DataPokupki from fio inner join korzina on fio.id_fio = korzina.id_fio) as korz inner join (select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie,img from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_tp_tp, Znachenie from tp_tp_type) as second on first.type = second.id_tp_tp) as tovars on korz.idtovar = tovars.id_Tovar";
			var SummaKorzina="select sum(price*kol_vo) as itogSumma from (select id_tovar,kol_vo,price,id_fio from korzina inner join (select id_Tovar as idTovar,price from tovar) as tovars on korzina.id_tovar = tovars.idTovar) as itog";
		}
		else
		{
			var Logged = "true";
			var zapros = "select @i:=@i+1 as num, id_fio,First_name,Last_name,id_tovar,id_korzina,Nazvanie,Opisanie,Znachenie,price,kol_vo,(price*kol_vo) as summ, NamePostavshik,DataPokupki,img from (select @i:=0, fio.id_fio,First_name,Last_name,id_korzina,id_tovar as idtovar,kol_vo,DataPokupki from fio inner join korzina on fio.id_fio = korzina.id_fio) as korz inner join (select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie,img from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_tp_tp, Znachenie from tp_tp_type) as second on first.type = second.id_tp_tp) as tovars on korz.idtovar = tovars.id_Tovar where id_fio = '"+req.session.UserId+"'";
			var SummaKorzina="select sum(price*kol_vo) as itogSumma from (select id_tovar,kol_vo,price,id_fio from korzina inner join (select id_Tovar as idTovar,price from tovar) as tovars on korzina.id_tovar = tovars.idTovar where id_fio = '"+req.session.UserId+"') as itog";
		}
		pool.query(zapros, 
			function(err, data) 
			{
				if(err) return console.log(err);
				console.log(data);
				
				pool.query(SummaKorzina, 
					function(err, data2) 
					{
						if(err) return console.log(err);
						console.log(data2[0].itogSumma);
						res.render("korzina.hbs", {
							tovars: data,
							isLogged: Logged,
							summa:data2[0].itogSumma
							});
					});
				
				
				
			});
	}
	else
	{
		var Logged = "false";
		res.redirect('/login');
	}
});


app.put("/korzina", urlencodedParser,function(req, res){
	var idkorzina = req.body.id;
	var kolvokorzina = req.body.kolvo;
	var zapros = "UPDATE `korzina` SET `kol_vo` = '"+kolvokorzina+"' WHERE `korzina`.`id_korzina` = "+idkorzina+" ";
	console.log(zapros);
	pool.query(zapros, 
				function(err, data) {
					if(err) return console.log(err);
					console.log(data);
					res.sendStatus(202);
				});
	
})


app.delete("/korzina", urlencodedParser,function(req, res){
	var idkorzina = req.body.id;
	var zapros = "DELETE FROM `korzina` WHERE `korzina`.`id_korzina` = " + idkorzina;
	console.log(zapros);
	pool.query(zapros, 
				function(err, data) {
					if(err) return console.log(err);
					console.log(data);
					res.sendStatus(202);
				});
	
})


app.get("/login", function(req, res){
    res.render("login.hbs");
});


app.get("/logout", function(req, res){
	console.log('logout', req.session);
	req.session.destroy(() => {
    res.redirect('/');
	});
});

app.get("/tovar",urlencodedParser, function(req,res){
	//let urlRequest = URL.parse(req.url, true);
	//console.log(urlRequest);
	if(typeof(req.session.UserId) != "undefined" && req.session.UserId !== null)
	{
		if(req.session.UserId==45)
		{
			var Logged = "admin";
		}
		else
		{
			var Logged = "true";
		}
	}
	else
	{
		var Logged = "false";
	}
	var id = req.query['id'];
	console.log(id);
	console.log(req.param);
	pool.query("select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie,img from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_tp_tp, Znachenie from tp_tp_type) as second on first.type = second.id_tp_tp where id_tovar = "+id, 
	function(err, data) {
		if(err) return console.log(err);
		//console.log(data);
		res.render("tovar.hbs", {
			tovar: data[0],
			isLogged: Logged
		});
    });
})

app.post("/tovar", urlencodedParser,function(req, res){
	if(typeof(req.session.UserId) != "undefined" && req.session.UserId !== null)
	{
		var UserId = req.session.UserId;
		var TovarId = req.body.id;
		var date;
		date = new Date();
		date = date.getUTCFullYear() + '-' +
			('00' + (date.getUTCMonth()+1)).slice(-2) + '-' +
			('00' + date.getUTCDate()).slice(-2) + ' ' + 
			('00' + date.getUTCHours()).slice(-2) + ':' + 
			('00' + date.getUTCMinutes()).slice(-2) + ':' + 
			('00' + date.getUTCSeconds()).slice(-2);
		console.log(date);
		var zapros = "INSERT INTO `korzina` (`id_korzina`, `id_fio`, `id_tovar`, `kol_vo`, `DataPokupki`) VALUES (NULL, '"+UserId+"', '"+TovarId+"', '1', '"+date+"')";
		console.log(zapros);
		pool.query(zapros, 
				function(err, data) {
					if(err) return console.log(err);
					console.log(data);
					res.sendStatus(201);
				});
	}
	else
	{
		res.send("Вы не авторизованы!");
	}
})

app.post("/login", urlencodedParser,function(req, res){
	console.log(req.body);
	if (req.body.action == "signin")
		{
			var email = req.body.email;
			var passw = req.body.password;
			var zapros = "select id_fio,First_name,Last_name from fio where mail = '"+ email +"' and password = '"+ passw +"'";
			pool.query(zapros, 
				function(err, data) {
					if(err) return console.log(err);
					console.log(data);
					if (data.length !== 0)
					{
						req.session.UserId=data[0].id_fio;
						console.log(data[0].id_fio);
						res.redirect("/korzina");
					}
					else res.sendStatus(401);
					//res.render("korzina.hbs", {
					//	tovars: data
				  //});
				});
		}
	if (req.body.action == "register")
	{
		var firstname = req.body.firstname;
		var lastname = req.body.lastname;
		var email = req.body.email;
		var passw = req.body.password;
		

			
		var checkzapros = "select * from fio where mail ='"+email+"'";
		pool.query(checkzapros, 
			function (err, data) {
				if(err) return console.log(err);
				console.log(data.length);
				if (data.length === 0)
					{
					var zapros = "INSERT INTO `fio` (`id_fio`, `First_name`, `Last_name`, `mail`, `password`) VALUES (NULL, '"+firstname+"', '"+lastname+"', '"+email+"', '"+passw+"')"
					console.log(zapros);
					pool.query(zapros, 
						function(err, data) {
							if(err) return console.log(err);
							console.log(data.insertId);
							var userId = data.insertId;
							req.session.UserId = userId;
							console.log(req.session);
							res.redirect("/");
							})
					}
				else 
					{
					res.sendStatus(401);
					}

				})
		
		
	}
	/*if(typeof(req.body.email) != "undefined" && req.body.email !== null) 
	{
	req.session.email = req.body.email;
	/*var zapros = "select id_fio,First_name,Last_name,id_tovar,id_korzina,Nazvanie,Opisanie,Znachenie,price,kol_vo,NamePostavshik,DataPokupki from (select fio.id_fio,First_name,Last_name,id_korzina,id_tovar as idtovar,kol_vo,DataPokupki from fio inner join korzina on fio.id_fio = korzina.id_fio) as korz inner join (select id_Tovar,Nazvanie,price,Name as NamePostavshik,Opisanie,Znachenie from (select * from tovar inner join postavshik on tovar.id_Postavshika = postavshik.id_Postavshik) as first inner join (select id_TP, Opisanie, Znachenie from tp_type inner join tp_tp_type on tp_type.type = tp_tp_type.id_tp_tp) as second on first.type = second.id_TP) as tovars on korz.idtovar = tovars.id_Tovar where First_name = '"+req.session.login+"'"
	pool.query(zapros, 
	function(err, data) {
		if(err) return console.log(err);
		console.log(data);
		//res.render("korzina.hbs", {
		//	tovars: data
      //});
    });
	console.log(req.body.action);
	console.log(req.session.email);
	//console.log(req.session);
	res.redirect("/");
	}
	else res.sendStatus(401);*/
})

app.post("/api/photo",urlencodedParser,function(req, res){
   let filedata = req.file;
    console.log(filedata);
    if(!filedata)
        res.send("Ошибка при загрузке файла");
    else
        res.send("Файл загружен");
   /*console.log(req.body);
	var body = '';

        req.on('data', function (data) {
            body += data;
			});
		req.on('end', function () {
            //var post = qs.parse(body);
            // use post['blah'], etc.
			//console.log(body);
			
			fs.writeFile('rx2.png', body, (err) => {
                if (err) throw err;
            })
			res.send("Ошибка при загрузке файла");
        });
	//console.log(body);
    /*if(!filedata)
	{
		console.log("Ошибка при загрузке файла");
        res.send("Ошибка при загрузке файла");
	}
    else
	{
		console.log("Файл загружен");
        res.send("Файл загружен");
	}*/
})


app.listen(3000, function(){
  console.log("Сервер ожидает подключения...");
});