"use client"

import { ModeToggle } from '@/components/mode-toggle'
import { Input } from '@/components/ui/input'
import { uploadFile } from '@/lib/uploadServices'
import { UserButton } from '@clerk/nextjs'
import { ChangeEvent } from 'react'

export default function Home() {

  const onChangeFile = async (event: ChangeEvent<HTMLInputElement>) =>{
    const files = event.target.files as FileList;

    const formData = new FormData();
    formData.append('file', files[0]);

    var res = await uploadFile(formData);

    console.log('up res',res)
  }

  return (
    <div>
      <UserButton afterSignOutUrl="/" />
      <ModeToggle/>
      <Input id="picture" type="file" onChange={onChangeFile}/>
      </div>
  )
}
