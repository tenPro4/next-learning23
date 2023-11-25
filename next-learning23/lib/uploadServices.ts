import { auth } from "@clerk/nextjs";
import { isTeacher } from "./teacher";
import axios from "axios";
import { NextResponse } from "next/server";

const uploadInstance = axios.create({
    baseURL:"https://localhost:7096/api"
})

// const handleAuth = () => {
//   const { userId } = auth();
//   const isAuthorized = isTeacher(userId);

//   if (!userId || !isAuthorized) throw new Error("Unauthorized");
//   return { userId };
// };

export const uploadVideo = async (data:FormData) =>{
    try{
        // if(!handleAuth()){
        //     return new NextResponse("Unauthorized", { status: 401 });
        // }

        var response = await uploadInstance.post("/files/video",data)

        return response;
    }
    catch(error){
        console.log("[Upload Video Fail]", error);
        return new NextResponse("Internal Error", { status: 500 });
    }
}

export const uploadFile = async (data:FormData) =>{
    try{
        // if(!handleAuth()){
        //     return new NextResponse("Unauthorized", { status: 401 });
        // }

        var response = await uploadInstance.post("/files/file",data)

        return response.data;
    }
    catch(error){
        console.log("[Upload Video Fail]", error);
        return new NextResponse("Internal Error", { status: 500 });
    }
}
